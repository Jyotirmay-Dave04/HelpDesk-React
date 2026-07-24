import { Category, ConfirmationNumber, Create, Dashboard, Group, Inbox, Schedule, type SvgIconComponent } from "@mui/icons-material";
import { UserRole } from "../types/user-role";

export interface NavItem {
    label: string;
    path: string;
    icon: SvgIconComponent;
    roles: UserRole[];
}

export const NAV_ITEMS: NavItem[] = [
    {
        label: 'Dashboard',
        path: '/dashboard',
        icon: Dashboard,
        roles: [UserRole.Admin, UserRole.Agent]
    },
    {
        label: 'All Tickets',
        path: '/allTickets',
        icon: ConfirmationNumber,
        roles: [UserRole.Admin]
    },
    {
        label: 'My Queue',
        path: '/myQueue',
        icon: Inbox,
        roles: [UserRole.Agent]
    },
    {
        label: 'My Tickets',
        path: '/myTickets',
        icon: ConfirmationNumber,
        roles: [UserRole.Admin, UserRole.Agent, UserRole.Requester]
    },
    {
        label: 'Users',
        path: '/userManagement',
        icon: Group,
        roles: [UserRole.Admin]
    },
    {
        label: 'Groups',
        path: '/groupManagement',
        icon: Category,
        roles: [UserRole.Admin]
    },
    {
        label: 'SLA Settings',
        path: '/slaSettings',
        icon: Schedule,
        roles: [UserRole.Admin]
    },
    {
        label: 'Create Ticket',
        path: '/ticket/create',
        icon: Create,
        roles: [UserRole.Requester]
    }
]