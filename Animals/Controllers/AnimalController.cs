using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Animals.Models;
using Newtonsoft.Json;
using System.Linq.Dynamic;

namespace Animals.Controllers
{
    
    public class AnimalController : Controller
    {
        private IAnimalRepository _repository;
        public AnimalController() { _repository = new AnimalRepository(); }
        public AnimalController(IAnimalRepository repository) { _repository = repository; }
        // GET: Animal
        public ActionResult Index()
        {
            
            return View(_repository.GetAnimals());
          
        }
        public ActionResult Search()
        {
            return PartialView();
        }

        //public JsonResult GetData(JqSearchIn si)
        public JsonResult GetData(JqSearchIn si)
        {
            int totalRecords;
            int startRow = (si.page * si.rows) + 1;
            int skip = (si.page > 0 ? si.page - 1 : 0) * si.rows;


            // а вот тут пора подключать System.Linq.Dynamic
            IQueryable<Animal> AnimalsHeap;
            
            if (si._search && !String.IsNullOrEmpty(si.filters))
            {
                var wc = si.GenerateWhereClause(typeof(Animal));

                AnimalsHeap = _repository.GetAnimalsHeap(wc.Clause, wc.FormatObjects);

                totalRecords = AnimalsHeap.Count();

                AnimalsHeap = AnimalsHeap
                    .OrderBy(si.sidx + " " + si.sord)
                    .Skip(skip)
                    .Take(si.rows);

            }
            else
            {
                //AnimalsHeap = db.Animals;

                //totalRecords = AnimalsHeap.Count();

                //AnimalsHeap = AnimalsHeap
                //    .OrderBy(si.sidx + " " + si.sord)
                //    .Skip(skip)
                //    .Take(si.rows);
                var heap = _repository.GetAnimals();
                int totalPagez = _repository.HowMuchAnimals();
                var result = new JqGridSearchOut
                {
                    total = totalPagez,
                    page = si.page,
                 //   records = totalRecords,
                    rows = (from als in heap
                            select new AnimalItemsJqGridRowOut()
                            {
                                Name = als.Name,
                                TypeName = als.Type.Name,
                                ColorName = als.Color.Name,
                                LocationName = als.Location.Name,
                                LocationRegionName = als.Location.Region.Name
                            }).ToArray()
                };

                return Json(result);
            }

            var AnimalsList = AnimalsHeap.ToList();
            
            var totalPages = (int)Math.Ceiling((float)totalRecords / si.rows);

            var grid = new JqGridSearchOut
            {
                total = totalPages,
                page = si.page,
                records = totalRecords,
                rows = (from als in AnimalsList
                        select new AnimalItemsJqGridRowOut()
                        {
                            Name = als.Name,
                            TypeName = als.Type.Name,
                            ColorName = als.Color.Name,
                            LocationName = als.Location.Name,
                            LocationRegionName = als.Location.Region.Name
                        }).ToArray()
            };

            return Json(grid);
        }


        //      Ниже свой велосипед под поиск с единичным условием. Т.к. нужен был множественный выбор
        //      с одновременным большим кол-вом условий, не используется. Вначале был написан, потом ещё раз прочитано задание.
        //public JsonResult GetData(int? page, bool _search, string Name, string TypeName, string ColorName, string LocationName, string LocationRegionName)
        //{
        //    int PageSize = 20;
        //    var list = db.Animals.AsQueryable();
        //    if (_search)
        //        {
        //             if (!String.IsNullOrEmpty(Name))
        //             list = list.Where(x => x.Name == Name);

        //             if (!String.IsNullOrEmpty(TypeName))
        //                 list = list.Where(x => x.Type.Name == TypeName);

        //             if (!String.IsNullOrEmpty(ColorName))
        //                 list = list.Where(x => x.Color.Name == ColorName);

        //             if (!String.IsNullOrEmpty(LocationName))
        //                 list = list.Where(x => x.Location.Name == LocationName);

        //             if (!String.IsNullOrEmpty(LocationRegionName))
        //                 list = list.Where(x => x.Location.Name == LocationRegionName);
        //        }
        //    // постраничная выборка данных 
        //    var data = list
        //        .OrderBy(x => x.Name)
        //        .Skip((page - 1 ?? 0) * PageSize)
        //        .Take(PageSize)
        //        .ToList();
        //    // формирование ответа в формате JSON
        //    var result = new JsonResult
        //    {
        //        Data = new
        //        {
        //            page,
        //            total = Math.Ceiling((double)list.Count() / PageSize),
        //            records = list.Count(),
        //            rows = data
        //        }
        //    };

        //    return result;
        //    //return JsonConvert.SerializeObject(db.Animals.ToList());
        //}

        // GET: Animal/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Animal animal = _repository.FindById(id);
            if (animal == null)
            {
                return HttpNotFound();
            }
            return View(animal);
        }

        // GET: Animal/Create
        public ActionResult Create()
        {
            PopulateColorsDropDownList();
            PopulateLocationDropDownList();
            PopulateTypeDropDownList();
            return View();
        }

        // POST: Animal/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,TypeId,ColorId,LocationId")] Animal animal)
        {
            if (ModelState.IsValid)
            {
                _repository.AddingAnimal(animal);
                return RedirectToAction("Index");
            }

            return View(animal);
        }

        // GET: Animal/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Animal animal = _repository.FindById(id);
            if (animal == null)
            {
                return HttpNotFound();
            }
            PopulateTypeDropDownList(animal);
            PopulateColorsDropDownList(animal);
            PopulateLocationDropDownList(animal);
            return PartialView(animal);
        }

        // POST: Animal/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,TypeId,ColorId,LocationId")] Animal animal)
        {
            if (ModelState.IsValid)
            {
                _repository.EditingAnimal(animal);
                return RedirectToAction("Index");
            }
            return View(animal);
        }

        // GET: Animal/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Animal animal = _repository.FindById(id);
            if (animal == null)
            {
                return HttpNotFound();
            }
            return View(animal);
        }

        // POST: Animal/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Animal animal = _repository.FindById(id);
            _repository.DeletingAnimal(animal);
            return RedirectToAction("Index");
        }

   
        //public JsonResult IsNameWasTyped(string name)
        //{
        //        return Json(name.Length > 2, JsonRequestBehavior.AllowGet);
        //}


        //Немного магии - строим выпадающие списки из родительских таблиц
        //Данный метод не смог достаточно строго унифицировать, потому пришлось наплодить под каждый класс
        private AnimalContext _db = new AnimalContext();
        private void PopulateColorsDropDownList ( object selectedColor = null)
        {
            var colorsQuery = (from c in _db.Colors
                               orderby c.Id
                               select c).ToList<Color>();
            ViewBag.ColorID = new SelectList(colorsQuery, "Id", "Name", selectedColor);
        }
        private void PopulateLocationDropDownList(object selectedLocation = null)
        {
            var LocationQuery = (from c in _db.Locations
                               orderby c.Id
                               select c).ToList<Location>();
            ViewBag.LocationID = new SelectList(LocationQuery, "Id", "Name", selectedLocation);
        }
        private void PopulateTypeDropDownList(object selectedType = null)
        {
            var TypesQuery = (from c in _db.Types
                                 orderby c.Id
                              select c).ToList<Animals.Models.Type>();                               //тащемта Type - имя системное, потому 
            ViewBag.TypeID = new SelectList(TypesQuery, "Id", "Name", selectedType);                //в данном случае обозвал через полный 
        }                                                                                          //путь пр-ва имен 

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
        
    }

}
