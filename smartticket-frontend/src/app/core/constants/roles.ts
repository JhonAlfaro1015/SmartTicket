export const ROLES = {
  Administrador: 'Administrador',
  Supervisor: 'Supervisor',
  Agente: 'Agente',
  Cliente: 'Cliente'
} as const;
 
export const ROLES_GESTION: readonly string[] = [ROLES.Administrador, ROLES.Supervisor];
export const ROLES_PERSONAL: string[] = [ROLES.Administrador, ROLES.Supervisor, ROLES.Agente];