import { TicketStatus } from "../types/ticket-status";
import { UserRole } from "../types/user-role";

export const ValidTransitions: Record<TicketStatus, TicketStatus[]> = {
    [TicketStatus.Open]: [TicketStatus.Assigned, TicketStatus.Rejected, TicketStatus.Cancelled],
    [TicketStatus.Assigned]: [TicketStatus.InProgress, TicketStatus.Rejected, TicketStatus.Cancelled],
    [TicketStatus.InProgress]: [TicketStatus.OnHold, TicketStatus.Resolved, TicketStatus.Cancelled, TicketStatus.CannotResolve],
    [TicketStatus.OnHold]: [TicketStatus.InProgress, TicketStatus.Resolved, TicketStatus.Cancelled, TicketStatus.CannotResolve],
    [TicketStatus.Resolved]: [TicketStatus.Closed, TicketStatus.ReOpen],
    [TicketStatus.Closed]: [],
    [TicketStatus.Rejected]: [],
    [TicketStatus.Cancelled]: [],
    [TicketStatus.ReOpen]: [TicketStatus.Assigned, TicketStatus.Cancelled, TicketStatus.InProgress],
    [TicketStatus.CannotResolve]: [TicketStatus.ReOpen],
};

export const RolePermissions: Record<UserRole, TicketStatus[]> = {
    [UserRole.Admin]: [
        TicketStatus.Assigned, TicketStatus.InProgress, TicketStatus.OnHold,
        TicketStatus.Resolved, TicketStatus.Closed, TicketStatus.Rejected,
        TicketStatus.Cancelled, TicketStatus.ReOpen, TicketStatus.CannotResolve,
    ],
    [UserRole.Agent]: [
        TicketStatus.InProgress, TicketStatus.OnHold, TicketStatus.Resolved,
        TicketStatus.Rejected, TicketStatus.Cancelled, TicketStatus.CannotResolve,
    ],
    [UserRole.Requester]: [TicketStatus.Cancelled, TicketStatus.ReOpen, TicketStatus.Closed],
};

const allowedTargetsForRole = (role: UserRole, current: TicketStatus): TicketStatus[] => {
    const validTargets = ValidTransitions[current] ?? [];
    const permitted = RolePermissions[role] ?? [];
    return validTargets.filter((t) => permitted.includes(t));
};

export const canAssign = (role: UserRole, current: TicketStatus) =>
    allowedTargetsForRole(role, current).includes(TicketStatus.Assigned);

export const canReject = (role: UserRole, current: TicketStatus) =>
    allowedTargetsForRole(role, current).includes(TicketStatus.Rejected);

export const canCancel = (role: UserRole, current: TicketStatus) =>
    allowedTargetsForRole(role, current).includes(TicketStatus.Cancelled);

export const canReopen = (role: UserRole, current: TicketStatus) =>
    allowedTargetsForRole(role, current).includes(TicketStatus.ReOpen);

export const canClose = (role: UserRole, current: TicketStatus) => 
    allowedTargetsForRole(role, current).includes(TicketStatus.Closed);

const DEDICATED_ACTION_STATUSES = [TicketStatus.Assigned, TicketStatus.Rejected, TicketStatus.Cancelled, TicketStatus.ReOpen, TicketStatus.Closed];

export const getDropdownTransitions = (role: UserRole, current: TicketStatus): TicketStatus[] =>
    allowedTargetsForRole(role, current).filter((t) => !DEDICATED_ACTION_STATUSES.includes(t));

export const requiresReason = (target: TicketStatus): boolean =>
    target === TicketStatus.CannotResolve || target === TicketStatus.OnHold;