﻿using System.Diagnostics.Metrics;
using System.Net;
using AutoMapper;
using AutoMapper.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProgrammingClub.DataContext;
using ProgrammingClub.Helpers;
using ProgrammingClub.Models;
using ProgrammingClub.Models.CreateModels;
using Member = ProgrammingClub.Models.Member;

namespace ProgrammingClub.Services
{
    public class ModeratorsService : IModeratorService
    {
        private readonly ProgrammingClubDataContext _context;
        private readonly IMembersService _membersService;
        private readonly IMapper _mapper;
        public ModeratorsService(
            ProgrammingClubDataContext context,
            IMembersService membersService,
            IMapper mapper)
        {
            _context = context;
            _membersService = membersService;
            _mapper = mapper;
        }

        public async Task CreateModerator(CreateModerator moderator)
        {
            if (!await _membersService.MemberExistByIdAsync(moderator.IDMember))
            {
                throw new Exception(ErrorMessagesEnum.ID_NotFound);
            }
            if (await GetModeratorByMemberID(moderator.IDMember) != null ) {
                throw new Exception(ErrorMessagesEnum.AlreadyExistsById);
            }
            var newModerator = _mapper.Map<Moderator>(moderator);
            newModerator.IDModerator = Guid.NewGuid();
            _context.Entry(newModerator).State = EntityState.Added;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteModerator(Guid id)
        {
            if (!await ModeratorExistByIdAsync(id))
                throw new Exception(ErrorMessagesEnum.Moderator_ID_NotFound);

            _context.Moderators.Remove(new Moderator { IDModerator = id });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Moderator?> GetModeratorById(Guid id)
        {
            if (!await ModeratorExistByIdAsync(id))
            {
                throw new Exception(ErrorMessagesEnum.Moderator_ID_NotFound);
            }
            return await _context.Moderators.FirstOrDefaultAsync(m => m.IDModerator == id);
        }

        public async Task<Moderator?> GetModeratorByMemberID(Guid? memberId)
        {
            if (memberId == null)
                return null;
            return await _context.Moderators.FirstOrDefaultAsync(m => m.IDMember == memberId);
        }

        public async Task<IEnumerable<Moderator>> GetModerators()
        {
             return await _context.Moderators.ToListAsync();
        }

        public async Task<Moderator?> UpdateModerator(Guid IDModerator, CreateModerator moderator)
        {
            var newModerator = _mapper.Map<Moderator>(moderator);
            await ValidateModerator(IDModerator, newModerator);
            newModerator.IDModerator = IDModerator;
            _context.Update(newModerator);
            await _context.SaveChangesAsync();
            return newModerator;
        }

        private async Task ValidateModerator(Guid IDModerator, Moderator moderator)
        {
            if (!await ModeratorExistByIdAsync(IDModerator))
            {
                throw new Exception(ErrorMessagesEnum.Moderator_ID_NotFound);
            }
            if (moderator.Description == null)
            {
                throw new Exception(ErrorMessagesEnum.EmptyField);
            }
            if (moderator.Title == null)
            {
                throw new Exception(ErrorMessagesEnum.EmptyField);
            }
            if (!await _membersService.MemberExistByIdAsync(moderator.IDMember))
            {
                throw new Exception(ErrorMessagesEnum.Member_ID_NotFound);
            }
        }

        public async Task<Moderator?> UpdatePartiallyModerator(Guid IDModerator, Moderator moderator)
        {
            var moderatorFromDatabase = await GetModeratorById(IDModerator);
            if (moderatorFromDatabase == null)
            {
                return null;
            }
            bool needUpdate = false;

            if (!string.IsNullOrEmpty(moderator.Title) && moderatorFromDatabase.Title != moderator.Title)
            {
                moderatorFromDatabase.Title = moderator.Title;
                needUpdate = true;

            }
            if (!string.IsNullOrEmpty(moderator.Description) && moderatorFromDatabase.Description != moderator.Description)
            {
                moderatorFromDatabase.Description = moderator.Description;
                needUpdate = true;
            }
            if (moderator.IDMember.HasValue && moderatorFromDatabase.IDMember != moderator.IDMember)
            {
                if (GetModeratorByMemberID(moderator.IDMember) != null)
                {
                    throw new Exception(ErrorMessagesEnum.Member_ID_NotFound);
                }
                moderatorFromDatabase.IDMember = moderator.IDMember;
                needUpdate = true;
            }
            if (!needUpdate)
            {
                throw new Exception(ErrorMessagesEnum.NoUpdates);
            }
            await ValidateModerator(IDModerator, moderatorFromDatabase);
            _context.Update(moderatorFromDatabase);
            await _context.SaveChangesAsync();
            return moderatorFromDatabase;
        }

        public async Task<bool> ModeratorExistByIdAsync(Guid? id)
        {
            if (id == null)
            {
                throw new Exception(ErrorMessagesEnum.Moderator_ID_NotFound);
            }
            return await _context.Moderators.CountAsync(moderator => moderator.IDModerator == id) > 0;
        }
    }
}
