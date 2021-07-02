using Appets.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appets.BusinessLogic.Interface
{
    public interface IPetLogic : ICrud<Pet>
    {
        void AddPost(Guid petId, Post post);
        Pet ChangePetStatusToFound(Guid id);
        IEnumerable<Pet> GetPetsByUser(Guid userId);
        Pet CreateWithUser(Pet pet, Guid ownerId);
    }
}
