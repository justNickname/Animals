using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Linq.Dynamic;

namespace Animals.Models
{
    // за ~80% кода скажем спасибо  пользователям stackoverflow.com
    public class JqSearchIn
    {
        public string sidx { get; set; }
        public string sord { get; set; }
        public int page { get; set; }
        public int rows { get; set; }
        public bool _search { get; set; }
        public string searchField { get; set; }
        public string searchOper { get; set; }
        public string searchString { get; set; }
        public string filters { get; set; }

        
        public WhereClause GenerateWhereClause(System.Type targetSearchType)
        {
            return new WhereClauseGenerator().Generate(_search, filters, targetSearchType);
        }
    
    }
    public class WhereClause
    {
        public string Clause { get; set; }
        public object[] FormatObjects { get; set; }
    }
    public class WhereClauseGenerator
    {

        private List<object> _formatObjects;

        public WhereClause Generate(bool _search, string filters, System.Type targetSearchType)
        {
            _formatObjects = new List<object>();



            return new WhereClause()
                         {
                             Clause = _search && !String.IsNullOrEmpty(filters) ?
                                        ParseFilter(new JavaScriptSerializer()
                                             .Deserialize<JqGridFilter>(filters), targetSearchType).ToString()
                                        : String.Empty,
                             FormatObjects = _formatObjects.ToArray()
                         };
        }
        private readonly string[] FormatMapping = {
            // 0 = field name,
            // 1 = total no of formats/params so far - 1
            // F = replaced by fmAdd
            "({0} = @{1}{F})",               // "eq" - equal
            "({0} <> @{1}{F})",              // "ne" - not equal
            "({0} < @{1}{F})",               // "lt" - less than
            "({0} <= @{1}{F})",              // "le" - less than or equal to
            "({0} > @{1}{F})",               // "gt" - greater than
            "({0} >= @{1}{F})",              // "ge" - greater than or equal to
            "({0}.StartsWith(@{1}){F})",     // "bw" - begins with
            "(!{0}.StartsWith(@{1}){F})",    // "bn" - does not begin with
            "({0}.EndsWith(@{1}){F})",       // "ew" - ends with
            "(!{0}.EndsWith(@{1}){F})",      // "en" - does not end with
            "({0}.Contains(@{1}){F})",       // "cn" - contains
            "(!{0}.Contains(@{1}){F})"       // "nc" - does not contain
        };
        private readonly string[] NullValueFormatMapping = {
            // 0 = field name
            "({0} = NULL)",     // "eq" - equal
            "({0} != NULL)",    // "ne" - not equal
            "(1=0)",            // "lt" - less than
            "(1=1)",            // "le" - less than or equal to
            "({0} != NULL)",    // "gt" - greater than
            "(1=1)",            // "ge" - greater than or equal to
            "({0} != NULL)",    // "bw" - begins with
            "({0} != NULL)",    // "bn" - does not begin with
            "({0} != NULL)",    // "ew" - ends with
            "({0} != NULL)",    // "en" - does not end with
            "({0} != NULL)",    // "cn" - contains
            "({0} != NULL)"     // "nc" - does not contain
        };
        private StringBuilder ParseRule(ICollection<JqGridRule> rules, GroupOp groupOp, System.Type targetSearchType)
        {
            if (rules == null || rules.Count == 0)
                return null;

            var sb = new StringBuilder();
            bool firstRule = true;
            var props = targetSearchType.GetProperties().ToDictionary(p => p.Name, p => p.PropertyType);

            foreach (var rule in rules)
            {
                if (!firstRule)
                    // skip groupOp before the first rule
                    sb.Append(groupOp);
                else
                    firstRule = false;

                // get the object type of the rule
                System.Type ruleParseType;
                bool emptyNullable = false;
                try
                {
                    System.Type ruleType = ruleParseType = props[rule.field];

                    if (ruleType.IsGenericType && ruleType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        if (rule.data == "")
                            emptyNullable = true;
                        ruleParseType = Nullable.GetUnderlyingType(ruleType);
                    }

                }
                catch (KeyNotFoundException)
                {
                    throw new ArgumentOutOfRangeException(rule.field + " is not a property of type "
                                                    + targetSearchType);
                }


                // parse it in as the correct object type
                var fmAdd = "";
                if (ruleParseType == typeof(string))
                {
                    _formatObjects.Add(rule.data);
                    if (rule.data == "")
                        fmAdd = " OR {0} = NULL";
                }
                else
                {
                    if (emptyNullable)
                        _formatObjects.Add(null);
                    else
                    {
                        var parseMethod = ruleParseType.GetMethod("Parse", new[] { typeof(string) });
                        if (parseMethod != null)
                            _formatObjects.Add(parseMethod.Invoke(props[rule.field], new object[] { rule.data }));
                        else
                            throw new ArgumentOutOfRangeException(rule.field +
                                                 " is not a string and cannot be parsed either!!");
                    }
                }

                string fm = emptyNullable ? NullValueFormatMapping[(int)rule.op]
                                            : FormatMapping[(int)rule.op].Replace("{F}", fmAdd);

                sb.AppendFormat(fm, rule.field, _formatObjects.Count - 1);

            }
            return sb.Length > 0 ? sb : null;
        }
        private void AppendWithBrackets(StringBuilder dest, StringBuilder src)
        {
            if (src == null || src.Length == 0)
                return;

            if (src.Length > 2 && src[0] != '(' && src[src.Length - 1] != ')')
            {
                dest.Append('(');
                dest.Append(src);
                dest.Append(')');
            }
            else
            {
                // verify that no other '(' and ')' exist in the b. so that
                // we have no case like src = "(x < 0) OR (y > 0)"
                for (int i = 1; i < src.Length - 1; i++)
                {
                    if (src[i] != '(' && src[i] != ')') continue;
                    dest.Append('(');
                    dest.Append(src);
                    dest.Append(')');
                    return;
                }
                dest.Append(src);
            }
        }
        private StringBuilder ParseFilter(ICollection<JqGridFilter> groups, GroupOp groupOp, System.Type targetSearchType)
        {
            if (groups == null || groups.Count == 0)
                return null;

            var sb = new StringBuilder();
            bool firstGroup = true;
            foreach (var group in groups)
            {
                var sbGroup = ParseFilter(group, targetSearchType);
                if (sbGroup == null || sbGroup.Length == 0)
                    continue;

                if (!firstGroup)
                    // skip groupOp before the first group
                    sb.Append(groupOp);
                else
                    firstGroup = false;

                sb.EnsureCapacity(sb.Length + sbGroup.Length + 2);
                AppendWithBrackets(sb, sbGroup);
            }
            return sb;
        }
        private StringBuilder ParseFilter(JqGridFilter filters, System.Type targetSearchType)
        {

            var parsedRules = ParseRule(filters.rules, filters.groupOp, targetSearchType);
            var parsedGroups = ParseFilter(filters.groups, filters.groupOp, targetSearchType);

            if (parsedRules != null && parsedRules.Length > 0)
            {
                if (parsedGroups != null && parsedGroups.Length > 0)
                {
                    var groupOpStr = filters.groupOp.ToString();
                    var sb = new StringBuilder(parsedRules.Length + parsedGroups.Length + groupOpStr.Length + 4);
                    AppendWithBrackets(sb, parsedRules);
                    sb.Append(groupOpStr);
                    AppendWithBrackets(sb, parsedGroups);
                    return sb;
                }
                return parsedRules;
            }
            return parsedGroups;
        }
    }
    public class AnimalItemsJqGridRowOut
    {
        public string Name { get; set; }
        public string TypeName { get ; set; }
        public string ColorName { get; set; }
        public string LocationName { get; set; }
        public string LocationRegionName { get; set; }
    }

    public class JqGridSearchOut
    {
        public int total { get; set; }
        public int page { get; set; }
        public int records { get; set; }
        public Array rows { get; set; }
    }

    public class JqGridFilter
    {
        public GroupOp groupOp { get; set; }
        public List<JqGridRule> rules { get; set; }
        public List<JqGridFilter> groups { get; set; }
    }

    public class JqGridRule
    {
        public string field { get; set; }
        public Operations op { get; set; }
        public string data { get; set; }
    }

    public enum GroupOp
    {
        AND,
        OR
    }

    public enum Operations
    {
        eq, // "equal"
        ne, // "not equal"
        lt, // "less"
        le, // "less or equal"
        gt, // "greater"
        ge, // "greater or equal"
        bw, // "begins with"
        bn, // "does not begin with"
        ew, // "ends with"
        en, // "does not end with"
        cn, // "contains"
        nc  // "does not contain"
        //in, // "in"
        //ni // "not in"
    }
}