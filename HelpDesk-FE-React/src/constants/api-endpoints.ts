import type { Priority } from "../types/priority";

export const ApiEndpoints = {
    AUTH: {
      LOGIN: '/Auth/login',
      REGISTER: '/Auth/register',
    },
    TICKETS: {
      BASE: '/Ticket',
      ALL: '/Ticket/getAll',
      GETALLWITHPOST: '/Ticket/getAllWithPost',
      BY_ID: (id: number) => `/Ticket/get/${id}`,
      ASSIGN: (id: number) => `/Ticket/${id}/assign`,
      STATUS: (id: number) => `/Ticket/${id}/status`,
      CREATE: '/Ticket/create',
      UPDATE: (id: number) => `/Ticket/update/${id}`,
      DELETE: (id: number) => `/Ticket/delete/${id}`,
      EXPORT: '/Ticket/export'
    },
    USERS: {
      BASE: '/User',
      ALL: '/User/getAll',
      BY_ID: (id: number) => `/User/${id}`,
      DELETE: (id: number) => `/User/delete/${id}`,
      CHANGE_ROLE: (id: number) => `/User/updateUserRole/${id}`,
      AGENTS: '/User/getAgents'
    },
    GROUPS: {
      BASE: '/Group/groups',
      PAGED: '/Group/groups/paged',
      CREATE: '/Group/groups/create',
      UPDATE: (id: number) => `/Group/groups/update/${id}`,
      DELETE: (id: number) => `/Group/groups/delete/${id}`
    },
    CATEGORIES: {
      BASE: '/Group/categories',
      BY_GROUP: (groupId: number) => `/Group/categories/group/${groupId}`,
      BY_GROUP_PAGED: (groupId: number) => `/Group/categories/group/${groupId}/paged`,
      CREATE: '/Group/categories/create',
      UPDATE: (id: number) => `/Group/categories/update/${id}`,
      DELETE: (id: number) => `/Group/categories/delete/${id}`
    },
    SUBCATEGORIES: {
      BY_CATEGORY: (categoryId: number) => `/Group/subcategories/category/${categoryId}`,
      BY_CATEGORY_PAGED: (categoryId: number) => `/Group/subcategories/category/${categoryId}/paged`,
      CREATE: '/Group/subcategories/create',
      UPDATE: (id: number) => `/Group/subcategories/update/${id}`,
      DELETE: (id: number) => `/Group/subcategories/delete/${id}`
    },
    COMMENTS: {
      BY_TICKET: (ticketId: number) => `/Comment/ticket/${ticketId}`,
      CREATE: '/Comment/create'
    },
    NOTIFICATIONS: {
      BASE: '/Notification',
      UNREAD_COUNT: '/Notification/unreadCount',
      MARK_READ: (id: number) => `/Notification/${id}/read`,
      MARK_ALL_READ: '/Notification/readAll'
    },
    ENUM: {
      PRIORITY: '/Enum/getPriorityEnums',
      TICKETSTATUS: '/Enum/getTicketStatusEnums',
      USERROLE: '/Enum/getUserRoleEnums',
      AUDITACTION: '/Enum/getAuditActionEnums'
    },
    AUDITLOGS: {
      BY_TICKET: (id: number) => `/AuditLog/ticket/${id}`,
    },
    DASHBOARD: {
      ADMIN_KPIS: '/Dashboard/adminKpis',
      CATEGORY_DISTRIBUTION: (view: string) => `/Dashboard/categoryDistribution?view=${view}`,
      TICKET_TRENDS: (view: string) => `/Dashboard/ticketTrends?view=${view}`,
      AGENT_METRICS: '/Dashboard/agentMetrics',
      STATS: '/Dashboard/stats',
      RECENT: '/Dashboard/recent'
    },
    SLAPOLICY: {
      BASE: '/SlaPolicy',
      UPDATE: (priority: Priority) => `/SlaPolicy/${priority}`
    },
    CANNED_RESPONSES: {
      BASE:   '/CannedResponse',
      CREATE: '/CannedResponse/create',
      UPDATE: (id: number) => `/CannedResponse/update/${id}`,
      DELETE: (id: number) => `/CannedResponse/delete/${id}`,
    }
  } as const;