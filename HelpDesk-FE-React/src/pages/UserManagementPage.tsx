import { useEffect, useRef, useState } from "react";
import { UserRole } from "../types/user-role";
import type { ChangeRolePayload, UserListItem } from "../interfaces/user";
import { DataGrid, type GridColDef, type GridSortModel } from "@mui/x-data-grid";
import { useDebouncedValue } from "../app/hooks/debounced-hook";
import { Box, Chip, FormControl, IconButton, InputAdornment, InputLabel, MenuItem, Paper, Select, TextField, Tooltip, Typography } from "@mui/material";
import dayjs from "dayjs";
import { Delete, ManageAccounts, Search } from "@mui/icons-material";
import { useAppSelector } from "../app/hooks/hooks";
import { useConfirm } from "../app/hooks/confirm-hook";
import { changeUserRoleAsync, deleteUserAsync, getAllUsersAsync } from "../services/user-service";
import { toast } from "../utils/Toast";
import { ChangeUserRoleDialog } from "../components/user-management/ChangeUserRoleDialog";

function UserManagementPage() {
    const [searchInput, setSearchInput] = useState('');
    const [roleFilter, setRoleFilter] = useState<UserRole | ''>('');
    const [roleDialogUser, setRoleDialogUser] = useState<UserListItem | null>(null);
    const [paginationModel, setPaginationModel] = useState({ page: 0, pageSize: 10 });
    const [sortModel, setSortModel] = useState<GridSortModel>([]);
    const debouncedSearch = useDebouncedValue(searchInput, 400);

    const [users, setUsers] = useState<UserListItem[]>([]);
    const [userCount, setUserCount] = useState(0);
    const [userLoading, setUserLoading] = useState(false);
    const [isUserRoleChanging, setIsUserRoleChanging] = useState(false);

    const { user: loggedUser } = useAppSelector(state => state.auth);
    const { confirm, ConfirmDialogUI } = useConfirm();

    const userListColums: GridColDef<UserListItem>[] = [
        { field: 'name', headerName: 'Name', minWidth: 170, flex: 1.2, filterable: false },
        { field: 'email', headerName: 'Email', minWidth: 230, flex: 1.5, filterable: false },
        {
            field: 'role', headerName: 'Role', minWidth: 130, flex: 1, filterable: false,
            renderCell: (params) => (
                <Chip
                    label={params.value}
                    size='small'
                    color={
                        params.value === UserRole.Admin ? 'primary'
                            : params.value === UserRole.Agent ? 'secondary'
                                : 'default'
                    }
                />
            )
        },
        {
            field: 'createdAt', headerName: 'Created At', minWidth: 150, flex: 1.2, filterable: false,
            valueFormatter: (value: string) => !value ? '—' : dayjs(value).format('DD MMM YYYY, hh:mm A')
        },
        {
            field: 'actions', headerName: 'Actions', width: 150, sortable: false, filterable: false,
            renderCell: (params) => (
                <>
                    <Tooltip title='Change Role'>
                        <IconButton color='primary' disabled={loggedUser.id === params.row.id} onClick={(e) => {
                            e.stopPropagation();
                            setRoleDialogUser(params.row);
                        }}>
                            <ManageAccounts></ManageAccounts>
                        </IconButton>
                    </Tooltip>

                    <Tooltip title='Delete'>
                        <IconButton color='error' disabled={loggedUser.id === params.row.id} onClick={(e) => {
                            e.stopPropagation();
                            deleteUser(params.row.id, params.row.name);
                        }}>
                            <Delete></Delete>
                        </IconButton>
                    </Tooltip>
                </>
            )
        }
    ];

    const prevFilterRef = useRef({ search: debouncedSearch, role: roleFilter });

    useEffect(() => {
        const filterchanged = prevFilterRef.current.search !== debouncedSearch || prevFilterRef.current.role !== roleFilter;

        prevFilterRef.current = { search: debouncedSearch, role: roleFilter };

        if (filterchanged && paginationModel.page !== 0) {
            setPaginationModel(prev => ({ ...prev, page: 0 }));
            return;
        }

        fetchUserList();
    }, [debouncedSearch, roleFilter, paginationModel, sortModel]);

    async function fetchUserList() {
        setUserLoading(true);
        try {
            const response = await getAllUsersAsync({
                search: debouncedSearch || undefined,
                role: roleFilter ? roleFilter : undefined,
                page: paginationModel.page + 1,
                pageSize: paginationModel.pageSize,
                sortBy: sortModel[0]?.field,
                sortDirection: sortModel[0]?.sort
            });
            setUsers(response.data.items);
            setUserCount(response.data.totalCount);
        } catch (error) {
            toast.error(error);
        } finally {
            setUserLoading(false);
        }
    }

    async function deleteUser(userId: number, userName: string) {
        const result = await confirm({
            title: "Delete User",
            message: `Are you sure you want to delete user - ${userName} ?`,
            confirmText: 'Delete'
        });

        if (!result.confirmed) return;

        try {
            const response = await deleteUserAsync(userId);
            toast.success(response.message);
            fetchUserList();
        } catch (error) {
            toast.error(error);
        }
    }

    const handleConfirmRoleChange = async (newRole: UserRole) => {
        if (!roleDialogUser) return;
        try {
            setIsUserRoleChanging(true);
            const response = await changeUserRoleAsync(roleDialogUser.id, { role: newRole } as ChangeRolePayload);
            toast.success(response.message);
            setRoleDialogUser(null);
            fetchUserList();
        } catch (error) {
            toast.error(error);
        } finally {
            setIsUserRoleChanging(false);
        }
    }

    return (
        <>
            <Box sx={{ p: 3 }}>
                <Typography variant='h5' sx={{ mb: 2 }}>User Management</Typography>

                <Box sx={{ display: 'flex', gap: 2, mb: 2 }}>
                    <TextField
                        placeholder='Search...'
                        value={searchInput}
                        onChange={(e) => {
                            setSearchInput(e.target.value);
                        }}
                        sx={{ minWidth: 360 }}
                        slotProps={{
                            input: {
                                startAdornment: (
                                    <InputAdornment position='start'>
                                        <Search />
                                    </InputAdornment>
                                )
                            }
                        }}
                    />

                    <FormControl sx={{ minWidth: 160 }}>
                        <InputLabel id='role-filter-label'>Role</InputLabel>
                        <Select
                            label="Role"
                            labelId="role-filter-label"
                            value={roleFilter}
                            onChange={(e) => {
                                setRoleFilter(e.target.value as UserRole | '');
                            }}
                        >
                            <MenuItem value={''}>All</MenuItem>
                            {Object.values(UserRole).map((role) => (
                                <MenuItem key={role} value={role}>{role}</MenuItem>
                            ))}
                        </Select>
                    </FormControl>
                </Box>

                <Paper>
                    <DataGrid
                        pagination
                        columns={userListColums}
                        rows={users}
                        loading={userLoading}
                        pageSizeOptions={[5, 10, 25, 50]}
                        paginationMode='server'
                        paginationModel={paginationModel}
                        onPaginationModelChange={(model) => {
                            setPaginationModel(prev => ({
                                page: model.pageSize != prev.pageSize ? 0 : model.page,
                                pageSize: model.pageSize
                            }))
                        }}
                        rowCount={userCount}
                        sortingMode='server'
                        sortModel={sortModel}
                        onSortModelChange={setSortModel}
                        checkboxSelection
                        disableRowSelectionOnClick
                    />
                </Paper>

                {roleDialogUser && (
                    <ChangeUserRoleDialog
                        open={!!roleDialogUser}
                        currentRole={roleDialogUser.role}
                        userName={roleDialogUser.name}
                        onClose={() => setRoleDialogUser(null)}
                        onConfirm={handleConfirmRoleChange}
                        loading={isUserRoleChanging}
                    />
                )}
            </Box>

            {ConfirmDialogUI}
        </>
    );
}

export default UserManagementPage;