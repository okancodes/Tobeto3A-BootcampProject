using Application.Features.Applicants.Constants;
using Application.Features.Applicants.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.Applicants.Constants.ApplicantsOperationClaims;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Applicants.Queries.GetById;

public class GetByIdApplicantQuery : IRequest<GetByIdApplicantResponse>, ISecuredRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetByIdApplicantQueryHandler : IRequestHandler<GetByIdApplicantQuery, GetByIdApplicantResponse>
    {
        private readonly IMapper _mapper;
        private readonly IApplicantRepository _applicantRepository;
        private readonly ApplicantBusinessRules _applicantBusinessRules;

        public GetByIdApplicantQueryHandler(IMapper mapper, IApplicantRepository applicantRepository, ApplicantBusinessRules applicantBusinessRules)
        {
            _mapper = mapper;
            _applicantRepository = applicantRepository;
            _applicantBusinessRules = applicantBusinessRules;
        }

        public async Task<GetByIdApplicantResponse> Handle(GetByIdApplicantQuery request, CancellationToken cancellationToken)
        {
            Applicant? applicant = await _applicantRepository.GetAsync(
                predicate: a => a.Id == request.Id, 
                include: x=>x.Include(x=>x.UserImages),
                cancellationToken: cancellationToken);
            await _applicantBusinessRules.ApplicantShouldExistWhenSelected(applicant);

            GetByIdApplicantResponse response = _mapper.Map<GetByIdApplicantResponse>(applicant);
            return response;
        }
    }
}