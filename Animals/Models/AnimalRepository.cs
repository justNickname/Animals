using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Dynamic;
using System.Data.Entity;

namespace Animals.Models
{
    public class AnimalRepository : IAnimalRepository
    {
        private AnimalContext _db = new AnimalContext();
        public IList<Animal> GetAnimals()
        {
            return _db.Animals.ToList();
        }
        public Animal FindById(int? id)
        {
            return _db.Animals.Find(id);
        }
        public IQueryable<Animal> GetAnimalsHeap(string clause, object[] clauseObj)
        {
            return _db.Animals.Where(clause, clauseObj);
        }
        public int HowMuchAnimals()
        {
            return _db.Animals.Count();
        }
        public void AddingAnimal(Animal animal)
        {
            _db.Animals.Add(animal);
            _db.SaveChanges();
        }
        public void EditingAnimal(Animal animal)
        {
            _db.Entry(animal).State = EntityState.Modified;
            _db.SaveChanges();
        }
        public void DeletingAnimal(Animal animal)
        {
            _db.Animals.Remove(animal);
            _db.SaveChanges();
        }

    }
}