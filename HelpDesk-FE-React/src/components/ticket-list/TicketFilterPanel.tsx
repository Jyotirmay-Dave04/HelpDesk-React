import { useFormik } from "formik";
import type { TicketFilter } from "../../interfaces/ticket";
import { UserRole } from "../../types/user-role";
import * as Yup from 'yup';
import type { AgentOption } from "../../interfaces/user";
import { Box, Button, Divider, Drawer, FormControl, IconButton, InputLabel, MenuItem, Select, Stack, TextField, Typography } from "@mui/material";
import { Priority } from "../../types/priority";
import { TicketStatus } from "../../types/ticket-status";
import AgentAutocomplete from "../ticket-details/AgentAutoComplete";
import { Close } from "@mui/icons-material";
import { useEffect, useState } from "react";
import type { Category, Group } from "../../interfaces/groups";
import { getCategories, getGroups } from "../../services/group-service";
import { toast } from "../../utils/Toast";

interface Props {
    open: boolean;
    role: UserRole;
    currentFilter: TicketFilter;
    currentAgent: AgentOption | null;
    onClose: () => void;
    onApply: (filter: TicketFilter, agent: AgentOption | null) => void;
}

const validationSchema = Yup.object({
    createdFrom: Yup.string().nullable(),
    createdTo: Yup.string()
        .nullable()
        .test('date-range', '"To" date must be after "From" date', function (value) {
            const { createdFrom } = this.parent;
            if (!createdFrom || !value) return true;
            return new Date(value) >= new Date(createdFrom);
        }),
});

const emptyTicketFilter = (page: number, pageSize: number) => {
    return {
        page,
        pageSize
    } as TicketFilter;
}

const TicketFilterPanel = ({ open, role, currentFilter, currentAgent, onClose, onApply }: Props) => {

    const [groups, setGroups] = useState<Group[]>([]);
    const [categories, setCategories] = useState<Category[]>([]);

    const formik = useFormik<TicketFilter & { assignedAgent?: AgentOption | null }>({
        initialValues: { ...currentFilter, assignedAgent: currentAgent },
        validationSchema,
        enableReinitialize: true,
        onSubmit: (values) => {
            const { assignedAgent, ...rest } = values;
            onApply({ ...rest, assignedTo: assignedAgent?.id, page: 1 }, assignedAgent ?? null);
            onClose();
        },
    });

    const handleReset = () => {
        const reset = emptyTicketFilter(1, currentFilter.pageSize);
        formik.setValues({ ...reset, assignedAgent: null });
        onApply(reset, null);
        onClose();
    };

    const isAdmin = role === UserRole.Admin;
    const isAgentOrAdmin = role === UserRole.Admin || role === UserRole.Agent;

    useEffect(() => {
        fetchGroups();
        return () => {
            setGroups([]);
        }
    }, []);

    async function fetchGroups() {
        try {
            const response = await getGroups();
            setGroups(response.data);
        } catch (error) {
            toast.error(error?.response?.data?.message ?? "Failed to load groups");
        }
    }

    useEffect(() => {
        if (formik.values.groupId) {
            fetchCategories(formik.values.groupId);
        }
        return () => {
            setCategories([]);
        }
    }, [formik.values.groupId]);

    async function fetchCategories(id: number) {
        try {
            const response = await getCategories(id);
            setCategories(response.data);
        } catch (error) {
            toast.error(error?.response?.data?.message ?? "Failed to load categories");
        }
    }

    return (
        <Drawer anchor="right" open={open} onClose={onClose}>
            <Box sx={{ width: 340, p: 3 }}>
                <Stack direction="row" sx={{ mb: 2, justifyContent: 'space-between', alignItems: 'center' }}>
                    <Typography variant="h6">Filters</Typography>
                    <IconButton onClick={onClose} size="small">
                        <Close />
                    </IconButton>
                </Stack>
                <Divider sx={{ mb: 3 }} />

                <Stack spacing={3} component="form" onSubmit={formik.handleSubmit}>
                    {/* Status — everyone sees this */}
                    <FormControl size="small" fullWidth>
                        <InputLabel id="filter-status-label">Status</InputLabel>
                        <Select
                            labelId="filter-status-label"
                            label="Status"
                            value={formik.values.status || ''}
                            onChange={(e) => formik.setFieldValue('status', e.target.value === '' ? undefined : e.target.value)}
                            onBlur={formik.handleBlur}
                        >
                            <MenuItem value={''}>All</MenuItem>
                            {Object.values(TicketStatus).map((status) => (
                                <MenuItem key={status} value={status}>
                                    {status}
                                </MenuItem>
                            ))}
                        </Select>
                    </FormControl>

                    {/* Priority — everyone sees this */}
                    <FormControl size="small" fullWidth>
                        <InputLabel id="filter-priority-label">Priority</InputLabel>
                        <Select
                            labelId="filter-priority-label"
                            label="Priority"
                            value={formik.values.priority || ''}
                            onChange={(e) => formik.setFieldValue('priority', e.target.value === '' ? undefined : e.target.value)}
                            onBlur={formik.handleBlur}
                        >
                            <MenuItem value={''}>All</MenuItem>
                            {Object.values(Priority).map((p) => (
                                <MenuItem key={p} value={p}>
                                    {p}
                                </MenuItem>
                            ))}
                        </Select>
                    </FormControl>

                    {/* Group — everyone sees this */}
                    <FormControl size="small" fullWidth>
                        <InputLabel id="filter-group-label">Group</InputLabel>
                        <Select
                            labelId="filter-group-label"
                            label="Group"
                            value={formik.values.groupId || ''}
                            onChange={(e) => {
                                formik.setFieldValue('groupId', !e.target.value ? undefined : e.target.value);
                                formik.setFieldValue('categoryId', undefined);
                                formik.setFieldTouched('categoryId', false);
                            }}
                            onBlur={formik.handleBlur}
                        >
                            <MenuItem value={''}>All</MenuItem>
                            {groups.map((p) => (
                                <MenuItem key={p.id} value={p.id}>
                                    {p.name}
                                </MenuItem>
                            ))}
                        </Select>
                    </FormControl>

                    {/* Category — everyone sees this */}
                    <FormControl size="small" fullWidth>
                        <InputLabel id="filter-category-label">Category</InputLabel>
                        <Select
                            labelId="filter-category-label"
                            label="Category"
                            value={formik.values.categoryId ?? ''}
                            onChange={(e) => formik.setFieldValue('categoryId', !e.target.value ? undefined : e.target.value)}
                            onBlur={formik.handleBlur}
                            disabled={!formik.values.groupId}
                        >
                            <MenuItem value={''}>All</MenuItem>
                            {categories.map((p) => (
                                <MenuItem key={p.id} value={p.id}>
                                    {p.name}
                                </MenuItem>
                            ))}
                        </Select>
                    </FormControl>

                    {isAgentOrAdmin && (
                        <FormControl size="small" fullWidth>
                            <InputLabel id="filter-sla-status-label">SLA Status</InputLabel>
                            <Select
                                labelId="filter-sla-status-label"
                                label="SLA Status"
                                value={formik.values.breached === undefined || formik.values.breached === null
                                    ? ''
                                    : String(formik.values.breached)
                                }
                                onChange={(e) => formik.setFieldValue('breached', e.target.value === '' ? undefined : e.target.value === 'true')}
                                onBlur={formik.handleBlur}
                            >
                                <MenuItem value={''}>All</MenuItem>
                                <MenuItem value={'true'}>Breached</MenuItem>
                                <MenuItem value={'false'}>Within SLA</MenuItem>
                            </Select>
                        </FormControl>
                    )}

                    {/* Assigned agent — Admin & Agent only, Requester doesn't need this */}
                    {isAdmin && (
                        <AgentAutocomplete
                            value={formik.values.assignedAgent ?? null}
                            onChange={(agent) => formik.setFieldValue('assignedAgent', agent)}
                        />
                    )}

                    <Divider />

                    {/* Date range — everyone sees this */}
                    <Typography variant="subtitle2">Created Date Range</Typography>
                    <Stack direction="row" spacing={2}>
                        <TextField
                            size="small"
                            label="From"
                            type="date"
                            fullWidth
                            value={formik.values.dateFrom ?? ''}
                            onChange={(e) => formik.setFieldValue('dateFrom', e.target.value || '')}
                            slotProps={{ inputLabel: { shrink: true } }}
                        />
                        <TextField
                            size="small"
                            label="To"
                            type="date"
                            fullWidth
                            value={formik.values.dateTo ?? ''}
                            onChange={(e) => formik.setFieldValue('dateTo', e.target.value || '')}
                            error={Boolean(formik.errors.dateTo)}
                            helperText={formik.errors.dateTo}
                            slotProps={{ inputLabel: { shrink: true } }}
                        />
                    </Stack>

                    <Stack direction="row" spacing={2} sx={{ mt: 2 }}>
                        <Button variant="outlined" fullWidth onClick={handleReset}>
                            Reset
                        </Button>
                        <Button variant="contained" fullWidth type="submit">
                            Apply
                        </Button>
                    </Stack>
                </Stack>
            </Box>
        </Drawer>
    );
};

export default TicketFilterPanel;