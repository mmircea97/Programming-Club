﻿
using ProgrammingClub.Models;
using ProgrammingClub.Models.CreateModels;

namespace ProgrammingClub.Services
{
    public interface IDropoutsService
    {
        public Task<IEnumerable<Dropout>> GetDropouts();
        public Task CreateDropout(CreateDropout dropout);
        public Task<bool> DeleteDropout(Guid id);
        public Task<Dropout?> UpdateDropout(Guid idDropout, CreateDropout dropout);
        public Task<Dropout?> UpdatePartiallyDropout(Guid idDropout, Dropout dropout);

        public Task<Dropout?> GetDropoutById(Guid id);
        Task<bool> DropoutExistByIdAsync(Guid? id);
    }
}
