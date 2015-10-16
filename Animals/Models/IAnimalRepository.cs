using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Animals.Models
{
    public interface IAnimalRepository
    {
        IList<Animal> GetAnimals();
        Animal FindById(int? id);
        IQueryable<Animal> GetAnimalsHeap(string clause, object[] clauseObj);
        int HowMuchAnimals();
        void AddingAnimal(Animal animal);
        void EditingAnimal(Animal animal);
        void DeletingAnimal(Animal animal);
        IList<Color> GetColorsQuery();
        IList<Location> GetLocationsQuery();
        IList<Type> GetTypesQuery();
        void GetDispose();
    }
}