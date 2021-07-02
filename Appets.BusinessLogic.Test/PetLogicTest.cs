using Appets.BusinessLogic.Interface;
using Appets.DataAccess.Interface;
using Appets.Domain;
using Appets.Domain.Enums;
using Appets.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Appets.BusinessLogic.Test
{
    [TestClass]
    public class PetLogicTest
    {
        private Pet pet;
        private Post post;
        private User owner;
        private Mock<IPetRepository> mockPetRepository;
        private Mock<IUserRepository> mockUserRepository;
        private IPetLogic service;
        [TestInitialize]
        public void SetUp()
        {
            pet = new Pet
            {
                Name = "laika",
                Type = "perra astronauta",
                Tags = new List<Tag>(),
                Owner = new User(),
                Posts = new List<Post>()
            };
            post = new Post
            {
                Title = "Post",
                Description = "This is a post",
                ReporterPhone = "098356232",
                Ubication = "Caramuru 1234",
                Type = PostTypeEnum.LOST
            };
            owner = new User
            {
                Id = new Guid("9A515BC6-6B6F-4D70-1A0B-08D88E834F3F"),
                Email = "juan@gmail.com",
                UserName = "jperez",
                Password = "perez123",
                Phone = "099999999",
                Pets = new List<Pet>(),
                Post = new List<Post>()
            };

            mockPetRepository = new Mock<IPetRepository>(MockBehavior.Strict);
            mockUserRepository = new Mock<IUserRepository>(MockBehavior.Strict);
            service = new PetLogic(mockPetRepository.Object, mockUserRepository.Object);
        }

        [TestMethod]
        public void AddPostTest()
        {
            mockPetRepository.Setup(m => m.Get(this.pet.Id)).Returns(this.pet);
            mockPetRepository.Setup(m => m.Get(this.pet.Id)).Returns(this.pet);
            mockPetRepository.Setup(m => m.Update(this.pet));
            mockPetRepository.Setup(m => m.Save());
            mockPetRepository.Setup(m => m.Get(this.pet.Id)).Returns(this.pet);

            service.AddPost(this.pet.Id, this.post);
            this.post.Description = "Edited Post";
            var updatedPet = service.Get(this.pet.Id);

            mockPetRepository.VerifyAll();
            Assert.AreEqual(updatedPet.Posts.Count, 1);
        }

        [TestMethod]
        public void CreatePet()
        {
            mockPetRepository.Setup(m => m.Add(this.pet));
            mockPetRepository.Setup(m => m.Save());

            Pet addedPet = service.Create(this.pet);

            mockPetRepository.VerifyAll();
            Assert.AreEqual(addedPet.Id, this.pet.Id);
        }

        [TestMethod]
        public void CreatePetWithOwner()
        {
            mockPetRepository.Setup(m => m.Add(pet));
            mockPetRepository.Setup(m => m.Save());
            mockUserRepository.Setup(m => m.Get(It.IsAny<Guid>()))
                .Returns(owner);

            var addedPet = service.CreateWithUser(pet, owner.Id);

            mockPetRepository.VerifyAll();
            mockUserRepository.VerifyAll();
            Assert.AreEqual(addedPet.Id, pet.Id);
            Assert.AreEqual(addedPet.Owner.Id, pet.Owner.Id);
            Assert.AreEqual(pet.Id, addedPet.Id);
            Assert.AreEqual(pet.Owner.Id, addedPet.Owner.Id);
        }


        [TestMethod]
        public void RemovePet()
        {
            mockPetRepository.Setup(m => m.Get(this.pet.Id)).Returns(this.pet);
            mockPetRepository.Setup(m => m.Remove(this.pet));
            mockPetRepository.Setup(m => m.Save());

            service.Remove(this.pet.Id);

            mockPetRepository.VerifyAll();
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void RemovePetNullId()
        {
            mockPetRepository.Setup(m => m.Get(this.pet.Id)).Returns(this.pet);
            mockPetRepository.Setup(m => m.Remove(this.pet)).Throws(new ArgumentException());

            service.Remove(Guid.Empty);
        }

        [TestMethod]
        public void UpdatePet()
        {
            Pet NewPet = new Pet
            {
                Name = "pet updated",
                Type = "tipo updated",
                Tags = new List<Tag>(),
                Owner = new User(),
                Posts = new List<Post>()
            };

            mockPetRepository.Setup(m => m.Get(this.pet.Id)).Returns(this.pet);
            mockPetRepository.Setup(m => m.Update(this.pet));
            mockPetRepository.Setup(m => m.Save());

            Pet updatedPet = service.Update(pet.Id, NewPet);

            mockPetRepository.VerifyAll();
            Assert.AreEqual(NewPet.Name, updatedPet.Name);
            Assert.AreEqual(NewPet.Type, updatedPet.Type);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void UpdatePetNullId()
        {
            mockPetRepository.Setup(m => m.Get(this.pet.Id)).Returns(this.pet);
            mockPetRepository.Setup(m => m.Update(this.pet)).Throws(new ArgumentException());

            service.Update(Guid.Empty, this.pet);
        }

        [ExpectedException(typeof(IncorrectParamException))]
        [TestMethod]
        public void UpdatePetInconsistentId()
        {
            Guid inconsistentGuid = Guid.NewGuid();
            mockPetRepository.Setup(m => m.Get(inconsistentGuid)).Throws(new IncorrectParamException("Id and object id doesnt match"));

            service.Update(inconsistentGuid, this.pet);
        }

        [TestMethod]
        public void GetAllPets()
        {
            List<Pet> listPets = new List<Pet>();
            listPets.Add(this.pet);

            mockPetRepository.Setup(m => m.GetAll()).Returns(listPets);

            List<Pet> result = service.GetAll().ToList();

            mockPetRepository.VerifyAll();
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void GetPetById()
        {
            mockPetRepository.Setup(m => m.Get(this.pet.Id)).Returns(this.pet);

            Pet result = service.Get(this.pet.Id);

            mockPetRepository.VerifyAll();
            Assert.AreEqual(result, this.pet);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void GetPetByIdNullId()
        {
            mockPetRepository.Setup(m => m.Get(It.IsAny<Guid>())).Throws(new ArgumentException());
            Pet result = service.Get(Guid.Empty);
            mockPetRepository.VerifyAll();
        }

        [TestMethod]
        public void GetPetByUserId()
        {
            pet.Owner = owner;
            mockPetRepository.Setup(m => m.GetAll())
                .Returns(new List<Pet> { pet });

            var petsByUser = service.GetPetsByUser(owner.Id);

            mockPetRepository.VerifyAll();
            Assert.AreEqual(pet.Id, petsByUser.FirstOrDefault().Id);
            Assert.AreEqual(pet.Owner.Id, petsByUser.FirstOrDefault().Owner.Id);
        }

        [TestMethod]
        public void ChangePetStatusToFound()
        {
            pet.Posts = new List<Post> { post };

            mockPetRepository.Setup(m => m.Get(It.IsAny<Guid>())).Returns(pet);
            mockPetRepository.Setup(m => m.Update(pet));
            mockPetRepository.Setup(m => m.Save());

            var petWithStatusChanged = service.ChangePetStatusToFound(owner.Id);

            mockPetRepository.VerifyAll();
            Assert.AreEqual(pet.Id, petWithStatusChanged.Id);
            Assert.AreEqual(pet.Posts.FirstOrDefault().Id, petWithStatusChanged.Posts.FirstOrDefault().Id);
        }
    }
}
