import { Box, Button, IconButton, Stack, Tooltip, Typography } from "@mui/material";
import { DataGrid, type GridColDef, type GridSortModel } from "@mui/x-data-grid";
import { useEffect, useState } from "react";
import type { TicketFilter } from "../interfaces/ticket";
import { toast } from "../utils/Toast";
import { UserRole } from "../types/user-role";
import { Delete, Edit, Visibility } from "@mui/icons-material";
import { useNavigate } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "../app/hooks/hooks";
import { deleteTicket, fetchTickets } from "../app/thunks/ticket-thunk";
import { useConfirm } from "../app/hooks/confirm-hook";
import { TicketStatus } from "../types/ticket-status";
import dayjs from "dayjs";
import { useDebouncedValue } from "../app/hooks/debounced-hook";
import TicketListToolbar from "../components/ticket-list/TicketListToolbar";
import TicketFilterPanel from "../components/ticket-list/TicketFilterPanel";
import type { AgentOption } from "../interfaces/user";

function TicketListPage() {
    const [paginationModel, setPaginationModel] = useState({ page: 0, pageSize: 10 });
    const [sortModel, setSortModel] = useState<GridSortModel>([]);
    const [filter, setFilter] = useState<TicketFilter>();
    const [selectedAgent, setSelectedAgent] = useState<AgentOption | null>(null);
    const [searchInput, setSearchInput] = useState('');
    const [filterPanelOpen, setFilterPanelOpen] = useState(false);
    const debouncedSearch = useDebouncedValue(searchInput, 400);

    const dispatch = useAppDispatch();
    const { user } = useAppSelector(state => state.auth);
    const { tickets, ticketCount, loading } = useAppSelector(state => state.ticket);
    const navigate = useNavigate();
    const { confirm, ConfirmDialogUI } = useConfirm();

    useEffect(() => {
        try {
            dispatch(fetchTickets({
                ...filter,
                page: paginationModel.page + 1,
                pageSize: paginationModel.pageSize,
                sortBy: sortModel[0]?.field,
                sortDirection: sortModel[0]?.sort,
            }));
        } catch (err) {
            toast.error(err);
        }
    }, [filter, paginationModel, sortModel]);

    useEffect(() => {
        setFilter((prev) => ({ ...prev, search: debouncedSearch || undefined, page: 1 }));
    }, [debouncedSearch]);

    const ticketListColumns: GridColDef[] = getColumnsBasedOnRole();

    function canEdit(status: TicketStatus) {
        return status === TicketStatus.Open || status === TicketStatus.Assigned;
    }

    function getColumnsBasedOnRole(): GridColDef[] {
        const base: GridColDef[] = [
            { field: 'serviceDetails', headerName: 'Service Details', width: 250, sortable: false, filterable: false },
            { field: 'status', headerName: 'Status', width: 120 },
            { field: 'priority', headerName: 'Priority', width: 120 },
            { field: 'groupName', headerName: 'Group', width: 140, sortable: false, filterable: false },
            { field: 'slaDeadline', headerName: 'SLA Deadline', width: 210, valueFormatter: (value: string) => !value ? '—' : dayjs(value).format('DD MMM YYYY, hh:mm A') },
            { field: 'resolvedAt', headerName: 'Resolved At', width: 210, valueFormatter: (value: string) => !value ? '—' : dayjs(value).format('DD MMM YYYY, hh:mm A') },
            { field: 'createdAt', headerName: 'Created At', width: 210, valueFormatter: (value: string) => !value ? '—' : dayjs(value).format('DD MMM YYYY, hh:mm A') }
        ];
        if (user.role === UserRole.Admin) return [
            ...base,
            { field: 'assignedToName', headerName: 'Assigned To', width: 150, sortable: false, filterable: false, valueFormatter: (value: string) => !value ? '—' : value },
            { field: 'requesterName', headerName: 'Requested By', width: 150, sortable: false, filterable: false, valueFormatter: (value: string) => !value ? '—' : value },
            {
                field: 'actions',
                headerName: 'Actions',
                width: 150,
                sortable: false,
                filterable: false,
                renderCell: (params) => (
                    <>
                        <Tooltip title='View'>
                            <IconButton color='primary' onClick={(e) => {
                                e.stopPropagation();
                                navigate(`/ticket/${params.row.id}`);
                            }}>
                                <Visibility></Visibility>
                            </IconButton>
                        </Tooltip>

                        {canEdit(params.row.status) && (
                            <Tooltip title='Edit'>
                                <IconButton color='primary' onClick={(e) => {
                                    e.stopPropagation();
                                    navigate(`/ticket/edit/${params.row.id}`);
                                }}>
                                    <Edit></Edit>
                                </IconButton>
                            </Tooltip>
                        )}

                        <Tooltip title='Delete'>
                            <IconButton color="error" onClick={(e) => {
                                e.stopPropagation();
                                ticketDelete(params.row.id, params.row.serviceDetails)
                            }}>
                                <Delete></Delete>
                            </IconButton>
                        </Tooltip>
                    </>
                )
            }
        ];
        if (user.role === UserRole.Agent) return [
            ...base,
            { field: 'requesterName', headerName: 'Requested By', width: 150, sortable: false, filterable: false, valueFormatter: (value: string) => !value ? '—' : value },
            {
                field: 'actions',
                headerName: 'Actions',
                width: 80,
                sortable: false,
                filterable: false,
                renderCell: (params) => (
                    <>
                        <Tooltip title='View'>
                            <IconButton color='primary' onClick={(e) => {
                                e.stopPropagation();
                                navigate(`/ticket/${params.row.id}`);
                            }}>
                                <Visibility></Visibility>
                            </IconButton>
                        </Tooltip>
                    </>
                )
            }
        ];
        return [
            ...base,
            {
                field: 'actions',
                headerName: 'Actions',
                width: 110,
                sortable: false,
                filterable: false,
                renderCell: (params) => (
                    <>
                        <Tooltip title='View'>
                            <IconButton color='primary' onClick={(e) => {
                                e.stopPropagation();
                                navigate(`/ticket/${params.row.id}`);
                            }}>
                                <Visibility></Visibility>
                            </IconButton>
                        </Tooltip>

                        {canEdit(params.row.status) && (
                            <Tooltip title='Edit'>
                                <IconButton color='primary' onClick={(e) => {
                                    e.stopPropagation();
                                    navigate(`/ticket/edit/${params.row.id}`);
                                }}>
                                    <Edit></Edit>
                                </IconButton>
                            </Tooltip>
                        )}
                    </>
                )
            }
        ];
    }

    async function ticketDelete(id: number, serviceDetails: string) {
        const result = await confirm({
            title: "Delete Ticket",
            message: `Are you sure you want to delete "${serviceDetails ?? `ticket #${id}`}"? This action cannot be undone.`,
            confirmText: "Delete"
        })

        if (!result.confirmed) return;

        try {
            const response = await dispatch(deleteTicket(id)).unwrap();
            toast.success(response.message);
            dispatch(fetchTickets({
                ...filter,
                page: paginationModel.page + 1,
                pageSize: paginationModel.pageSize,
                sortBy: sortModel[0]?.field,
                sortDirection: sortModel[0]?.sort,
            }));
        } catch (err) {
            toast.error(err);
        }
    }

    const activeFilterCount = countActiveFilters(filter);

    function countActiveFilters(filter: TicketFilter): number {
        let count = 0;
        if (filter?.status) count++;
        if (filter?.priority) count++;
        if (filter?.groupId) count++;
        if (filter?.categoryId) count++;
        if (filter?.assignedTo) count++;
        if (filter?.breached) count++;
        if (filter?.dateFrom || filter?.dateTo) count++;
        return count;
    }

    return (
        <>
            <Box sx={{ maxWidth: '100%'}}>
                <Stack direction='row' sx={{ display: 'flex', justifyContent: 'space-between' }}>
                    <Typography variant="h5" sx={{ my: 2 }}>Tickets</Typography>
                    <Box>
                        <Button variant='contained' size='small' onClick={() => navigate('/ticket/create')} sx={{ my: 2, mx: 1 }}>Create Ticket</Button>
                    </Box>
                </Stack>

                <TicketListToolbar
                    searchValue={searchInput}
                    onSearchChange={setSearchInput}
                    activeFilterCount={activeFilterCount}
                    onOpenFilterPanel={() => setFilterPanelOpen(true)}
                />

                <DataGrid
                    pagination
                    columns={ticketListColumns}
                    rows={tickets}
                    loading={loading}
                    pageSizeOptions={[5, 10, 25, 50]}
                    paginationMode='server'
                    paginationModel={paginationModel}
                    onPaginationModelChange={(model) => {
                        setPaginationModel(prev => ({
                            page: model.pageSize !== prev.pageSize ? 0 : model.page,
                            pageSize: model.pageSize
                        }))
                    }}
                    rowCount={ticketCount}
                    sortingMode="server"
                    sortModel={sortModel}
                    onSortModelChange={setSortModel}
                    checkboxSelection
                    disableRowSelectionOnClick
                />

                <TicketFilterPanel
                    open={filterPanelOpen}
                    role={user.role as UserRole}
                    currentFilter={filter}
                    currentAgent={selectedAgent}
                    onClose={() => setFilterPanelOpen(false)}
                    onApply={(applied, agent) => {
                        setFilter({ ...applied, search: filter.search });
                        setSelectedAgent(agent ?? null);
                    }}
                />
            </Box>

            {ConfirmDialogUI}
        </>
    );
}

export default TicketListPage;