using Appets.BusinessLogic.Interface;
using Appets.DataAccess.Interface;
using Appets.Domain;
using Appets.Domain.Enums;
using Appets.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appets.BusinessLogic
{
    public class PetLogic : IPetLogic
    {
        private IRepository<Pet> Repository;

        private IUserRepository UserRepository;

        public PetLogic(IRepository<Pet> repository, IUserRepository userRepository)
        {
            this.Repository = repository;
            this.UserRepository = userRepository;
        }

        public Pet Create(Pet pet)
        {
            Pet newPet = new Pet()
            {
                Name = pet.Name,
                Type = pet.Type,
                Image = pet.Image,
                Tags = pet.Tags,
                Owner = pet.Owner,
                Posts = pet.Posts
            };
            Repository.Add(newPet);
            Repository.Save();
            return newPet;
        }

        public Pet Get(Guid id)
        {
            return Repository.Get(id);
        }

        public IEnumerable<Pet> GetAll() => Repository.GetAll();

        public void Remove(Guid id)
        {
            Pet petFinded = Repository.Get(id);
            Repository.Remove(petFinded);
            Repository.Save();
        }

        public Pet Update(Guid id, Pet pet)
        {
            if (id != pet.Id) throw new IncorrectParamException("Id and object id doesnt match");

            Pet petToUpdate = this.Repository.Get(id);

            petToUpdate.Name = pet.Name;
            petToUpdate.Type = pet.Type;
            petToUpdate.Tags = pet.Tags;
            petToUpdate.Owner = pet.Owner;
            petToUpdate.Posts = pet.Posts;

            Repository.Update(petToUpdate);
            Repository.Save();
            return pet;
        }

        public void AddPost(Guid petId, Post post)
        {
            Pet pet = this.Get(petId);
            if (pet.Posts == null) pet.Posts = new List<Post>();
            pet.Posts.Add(post);
            this.Update(pet.Id, pet);
        }

        public Pet ChangePetStatusToFound(Guid id)
        {
            Pet pet = this.Get(id);
            if (pet.Posts != null)
            {
                ChangePetStatus(pet);
            }
            return this.Update(pet.Id, pet);
        }

        private static void ChangePetStatus(Pet pet)
        {
            bool finish = false;
            for (int i = 0; i < pet.Posts.Count && !finish; i++)
            {
                if (pet.Posts[i].Type == PostTypeEnum.LOST)
                {
                    pet.Posts[i].Type = PostTypeEnum.FOUND;
                    finish = true;
                }
            }
        }

        public IEnumerable<Pet> GetPetsByUser(Guid userId)
        {
            return Repository.GetAll().Where(p => p.Owner != null && p.Owner.Id == userId && !p.IsDeleted);
        }

        public Pet CreateWithUser(Pet pet, Guid ownerId)
        {
            User user = UserRepository.Get(ownerId);
            pet.Owner = user;
            return this.Create(pet);

        }
    }
}
