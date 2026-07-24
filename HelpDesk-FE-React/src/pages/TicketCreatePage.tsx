import { Box, Button, MenuItem, Stack, TextField, Typography } from "@mui/material";
import { useFormik } from "formik";
import type { TicketCreate } from "../interfaces/ticket";
import * as Yup from 'yup';
import { Priority } from "../types/priority";
import { useAppDispatch, useAppSelector } from "../app/hooks/hooks";
import { useEffect, useMemo, useState } from "react";
import { toast } from "../utils/Toast";
import { createTicket, fetchTicketById, updateTicket } from "../app/thunks/ticket-thunk";
import { useNavigate, useParams } from "react-router-dom";
import type { Category, Group, SubCategory } from "../interfaces/groups";
import { getCategories, getGroups, getSubCategories } from "../services/group-service";
import { clearSelectedTicket } from "../app/slices/ticket-slice";

const emptyValues: TicketCreate = {
    groupId: '',
    categoryId: '',
    subCategoryId: '',
    priority: '',
    serviceDetails: ''
};

function CreateTicketPage() {
    const { id } = useParams();
    const isEditMode = Boolean(id);
    const ticketId = id ? Number(id) : undefined;

    const [groups, setGroups] = useState<Group[]>([]);
    const [categories, setCategories] = useState<Category[]>([]);
    const [subCategories, setSubCategories] = useState<SubCategory[]>([]);

    const dispatch = useAppDispatch();
    const ticketSelector = useAppSelector(state => state.ticket);
    const navigate = useNavigate();

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
        try {
            if (isEditMode && ticketId) {
                dispatch(fetchTicketById(ticketId));
            }
        } catch (error) {
            toast.error(error);
        }
        return () => {
            dispatch(clearSelectedTicket());
        }
    }, [isEditMode, ticketId])


    const initialValues: TicketCreate = useMemo(() => {
        if (isEditMode && ticketSelector.selectedTicket) {
            return {
                groupId: ticketSelector.selectedTicket.groupId,
                categoryId: ticketSelector.selectedTicket.categoryId,
                subCategoryId: ticketSelector.selectedTicket.subCategoryId,
                priority: ticketSelector.selectedTicket.priority,
                serviceDetails: ticketSelector.selectedTicket.serviceDetails
            };
        }
        return emptyValues;
    }, [isEditMode, ticketSelector.selectedTicket])

    const formik = useFormik({
        initialValues,
        enableReinitialize: true,
        validationSchema: Yup.object({
            groupId: Yup.number().required('Group is required'),
            categoryId: Yup.number().required('Category is required'),
            subCategoryId: Yup.number().required('Sub category is required'),
            priority: Yup.mixed<Priority>().oneOf(Object.values(Priority)).required('Priority is required'),
            serviceDetails: Yup.string().required('Service details is required')
                .min(5, 'Details must have at least 5 characters')
                .max(1000, 'Datails must be less than 1000 characters')
        }),
        onSubmit: async (values, { setSubmitting }) => {
            try {
                if (isEditMode && ticketId) {
                    if (values.priority === ticketSelector.selectedTicket.priority
                        && values.serviceDetails === ticketSelector.selectedTicket.serviceDetails) {
                        return;
                    }
                }
                const response = (isEditMode && ticketId && ticketSelector.selectedTicket)
                    ? await dispatch(updateTicket({
                        id: ticketId,
                        body: values,
                    })).unwrap()
                    : await dispatch(createTicket(values)).unwrap();

                toast.success(response.message);
                navigate(-1);
            } catch (err) {
                toast.error(err);
            }
            setSubmitting(false);
        }
    });

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

    useEffect(() => {
        if (formik.values.categoryId)
            fetchSubCategories(formik.values.categoryId);

        return () => {
            setSubCategories([]);
        }
    }, [formik.values.categoryId]);

    async function fetchSubCategories(id: number) {
        try {
            const response = await getSubCategories(id);
            setSubCategories(response.data);
        } catch (error) {
            toast.error(error?.response?.data?.message ?? "Failed to load sub categories");
        }
    }

    return (
        <>
            <Box component='form' onSubmit={formik.handleSubmit} sx={{ maxWidth: 700 }}>
                <Stack direction='column' spacing={4}>
                    <Typography variant='h4' sx={{ my: 2 }} >{isEditMode ? 'Edit' : 'Create'} Ticket</Typography>

                    <TextField
                        select
                        margin='normal'
                        label="Group"
                        name='groupId'
                        id="group-id-select"
                        value={formik.values.groupId}
                        onChange={(e) => {
                            formik.setFieldValue('groupId', e.target.value);
                            formik.setFieldValue('categoryId', '');
                            formik.setFieldValue('subCategoryId', '');
                            formik.setFieldTouched('categoryId', false);
                            formik.setFieldTouched('subCategoryId', false);
                        }}
                        onBlur={formik.handleBlur}
                        error={formik.touched.groupId && !!formik.errors.groupId}
                        helperText={formik.touched.groupId && formik.errors.groupId}
                        disabled={isEditMode && !!ticketId}
                    >
                        <MenuItem value=''>Select Group</MenuItem>
                        {groups.map((g) => (
                            <MenuItem key={g.id} value={g.id}>
                                {g.name}
                            </MenuItem>
                        ))}
                    </TextField>

                    <TextField
                        select
                        margin='normal'
                        label='Category'
                        name="categoryId"
                        id="category-id-select"
                        value={formik.values.categoryId}
                        onChange={(e) => {
                            formik.setFieldValue('categoryId', e.target.value);
                            formik.setFieldValue('subCategoryId', '');
                            formik.setFieldTouched('subCategoryId', false);
                        }}
                        onBlur={formik.handleBlur}
                        disabled={!formik.values.groupId || (isEditMode && !!ticketId)}
                        error={formik.touched.categoryId && !!formik.errors.categoryId}
                        helperText={formik.touched.categoryId && formik.errors.categoryId}
                    >
                        <MenuItem value=''>Select Category</MenuItem>
                        {categories.map((c) => (
                            <MenuItem key={c.id} value={c.id}>
                                {c.name}
                            </MenuItem>
                        ))}
                    </TextField>

                    <TextField
                        select
                        margin='normal'
                        label='Sub Category'
                        name="subCategoryId"
                        id="sub-category-id-select"
                        value={formik.values.subCategoryId}
                        onChange={formik.handleChange}
                        onBlur={formik.handleBlur}
                        disabled={(!formik.values.groupId && !formik.values.categoryId) || (isEditMode && !!ticketId)}
                        error={formik.touched.subCategoryId && !!formik.errors.subCategoryId}
                        helperText={formik.touched.subCategoryId && formik.errors.subCategoryId}
                    >
                        <MenuItem value=''>Select Sub Category</MenuItem>
                        {subCategories.map((s) => (
                            <MenuItem key={s.id} value={s.id}>
                                {s.name}
                            </MenuItem>
                        ))}
                    </TextField>

                    <TextField
                        select
                        margin="normal"
                        label="Priority"
                        name="priority"
                        id="priority-select"
                        value={formik.values.priority}
                        onChange={formik.handleChange}
                        onBlur={formik.handleBlur}
                        error={formik.touched.priority && !!formik.errors.priority}
                        helperText={formik.touched.priority && formik.errors.priority}
                    >
                        <MenuItem value=''>Select Priority</MenuItem>
                        {Object.values(Priority).map(p => (
                            <MenuItem key={p} value={p}>
                                {p}
                            </MenuItem>
                        ))}
                    </TextField>

                    <TextField
                        multiline
                        rows={3}
                        margin="normal"
                        label="Service Details"
                        name="serviceDetails"
                        value={formik.values.serviceDetails}
                        onChange={formik.handleChange}
                        onBlur={formik.handleBlur}
                        error={formik.touched.serviceDetails && !!formik.errors.serviceDetails}
                        helperText={formik.touched.serviceDetails && formik.errors.serviceDetails}
                    />

                    <Stack direction='row' spacing={3} sx={{ justifyContent: 'flex-end' }}>
                        <Button type='button' variant='outlined' onClick={() => navigate(-1)}>
                            Cancel
                        </Button>
                        <Button loading={formik.isSubmitting} type='submit' variant='contained'
                            disabled={formik.isSubmitting || (isEditMode && ticketSelector.selectedTicket && formik.values.priority === ticketSelector.selectedTicket.priority
                                && formik.values.serviceDetails === ticketSelector.selectedTicket.serviceDetails)}>
                            Submit
                        </Button>
                    </Stack>
                </Stack>
            </Box>
        </>
    );
}

export default CreateTicketPage;