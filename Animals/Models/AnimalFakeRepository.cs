using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Animals.Models
{
    public class AnimalFakeRepository : IAnimalRepository
    {
        private List<Animal> _likeDb = new List<Animal>();
         IList<Animal> IAnimalRepository.GetAnimals()
        {
            return new List<Animal> { new Animal {Id = 100500, Name = "Evpatiy", TypeID = 2, ColorID = 2, LocationID = 2, Type = new Type{ Id = 2, Name = "Cheshirsiy kotofey"}, 
                Color = new Color{ Id = 2, Name = "Sero-polosatiy"}, Location = new Location{ Id = 2, Name = "Vannaya", RegionID = 2, Region = new Region{ Id = 2, Name = "Kvartira"}}}};
        }

        Animal IAnimalRepository.FindById(int? id)
        {
            return new Animal
            {
                Id = 100500,
                Name = "Evpatiy",
                TypeID = 2,
                ColorID = 2,
                LocationID = 2,
                Type = new Type { Id = 2, Name = "Cheshirsiy kotofey" },
                Color = new Color { Id = 2, Name = "Sero-polosatiy" },
                Location = new Location { Id = 2, Name = "Vannaya", RegionID = 2, Region = new Region { Id = 2, Name = "Kvartira" } }
            }; 
        }

        IQueryable<Animal> IAnimalRepository.GetAnimalsHeap(string clause, object[] clauseObj)
        {
           return new List<Animal> {new Animal
            {
                Id = 100500,
                Name = "Evpatiy",
                TypeID = 2,
                ColorID = 2,
                LocationID = 2,
                Type = new Type { Id = 2, Name = "Cheshirsiy kotofey" },
                Color = new Color { Id = 2, Name = "Sero-polosatiy" },
                Location = new Location { Id = 2, Name = "Vannaya", RegionID = 2, Region = new Region { Id = 2, Name = "Kvartira" } } } 
            } as IQueryable<Animal>; 
        }

        int IAnimalRepository.HowMuchAnimals()
        {
            return 1;
        }

        void IAnimalRepository.AddingAnimal(Animal animal)
        {
            _likeDb.Add(animal);
        }

        void IAnimalRepository.EditingAnimal(Animal animal)
        {
            _likeDb.Add(animal);
        }

        void IAnimalRepository.DeletingAnimal(Animal animal)
        {
            _likeDb.Remove(animal);
        }




        IList<Color> IAnimalRepository.GetColorsQuery()
        {
            return new List<Color> { new Color { Id = 2, Name = "Sero-polosatiy" } };
        }

        IList<Location> IAnimalRepository.GetLocationsQuery()
        {
            return new List<Location> { new Location { Id = 2, Name = "Vannaya", RegionID = 2, Region = new Region { Id = 2, Name = "Kvartira" }}};
        }

        IList<Type> IAnimalRepository.GetTypesQuery()
        {
            return new List<Type> { new Type { Id = 2, Name = "Sero-polosatiy" }};
        }

        void IAnimalRepository.GetDispose()
        {
            throw new NotImplementedException();
        }
    }
}