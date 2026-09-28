using AutoMapper;
using SmartTicket.Application.Dtos.Auth;
using SmartTicket.Application.Dtos.Auth.LoginRequestDto;
using SmartTicket.Application.Dtos.Clientes;
using SmartTicket.Application.Dtos.Solicitudes;
using SmartTicket.Application.Features.Auth.Login;
using SmartTicket.Application.Features.Clientes.Auth;
using SmartTicket.Application.Features.Clientes.Create;
using SmartTicket.Application.Features.Solicitudes.Create;
using SmartTicket.Domain.Entity;

namespace SmartTicket.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<LoginRequestDto, LoginCommand>();
            CreateMap<RegisterRequestDto, RegisterCommand>();
            CreateMap<CreateClienteDto, CreateClienteCommand>();
            CreateMap<Tbl_Cliente, ClienteResponseDto>();
            CreateMap<CreateSolicitudDto, CreateSolicitudCommand>();
            CreateMap<Tbl_Solicitud, SolicitudResponseDto>();
            CreateMap<ClienteRegisterDto, RegisterClienteCommand>();
            CreateMap<ClienteLoginDto, LoginClienteCommand>();
        }
    }
}
