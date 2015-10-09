using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Animals.Models
{
    public class Animal
    {
        public int Id { get; set; }
        
        //Даем полю класса имя, задаем свойства RemoteAttribute для удобной валидации
         //[Remote("IsNameWasTyped", "Animal", ErrorMessage = "Nope. The typed name is too small ")]
        //А нет, не внимательно прочет задание - валидаторы должны работать сразу по событию (т.е. на стороне клиента), 
        //а не через запрос, как работает RemoteAttribute (проверка идет на стороне сервера).
       
        [Display(Name = "Имя")]
        [StringLength(15, MinimumLength = 3)]
        public string Name { get; set; }
        public int TypeID { get; set; }
        public int ColorID { get; set; }
        public int LocationID { get; set; }

        // далее определено navigation property для использования "ленивой загрузки" в entity framework

        public virtual Type Type { get; set; }
        public virtual Color Color { get; set; }
        public virtual Location Location { get; set; }
    }
}