import { Box, Typography, Stack } from "@mui/material";
import { useState, useCallback, useEffect } from "react";
import { useConfirm } from "../app/hooks/confirm-hook";
import EntityColumn from "../components/group-management/EntityColumn";
import EntityFormDialog from "../components/group-management/EntityFormDialog";
import type { EntityLevel, EntityFormValues, Group, Category, SubCategory } from "../interfaces/groups";
import { toast } from "../utils/Toast";
import { createCategory, createGroup, createSubCategory, deleteCategory, deleteGroup, deleteSubCategory, getPagedCategories, getPagedGroups, getPagedSubCategories, updateCategory, updateGroup, updateSubCategory } from "../services/group-service";

const GROUP_PAGE_SIZE = 10;
const CATEGORY_PAGE_SIZE = 10;
const SUBCATEGORY_PAGE_SIZE = 10;
 
interface DialogState {
    open: boolean;
    level: EntityLevel;
    mode: "create" | "edit";
    editingId: number | null;
    initialValues: EntityFormValues;
}
 
const emptyValues: EntityFormValues = { name: "" };
 
function GroupManagementPage() {
    const [groups, setGroups] = useState<Group[]>([]);
    const [categories, setCategories] = useState<Category[]>([]);
    const [subCategories, setSubCategories] = useState<SubCategory[]>([]);
 
    const [groupsLoading, setGroupsLoading] = useState(false);
    const [categoriesLoading, setCategoriesLoading] = useState(false);
    const [subCategoriesLoading, setSubCategoriesLoading] = useState(false);
 
    const [groupPage, setGroupPage] = useState(1);
    const [groupTotalPages, setGroupTotalPages] = useState(1);
    const [categoryPage, setCategoryPage] = useState(1);
    const [categoryTotalPages, setCategoryTotalPages] = useState(1);
    const [subCategoryPage, setSubCategoryPage] = useState(1);
    const [subCategoryTotalPages, setSubCategoryTotalPages] = useState(1);
 
    const [selectedGroupId, setSelectedGroupId] = useState<number | null>(null);
    const [selectedCategoryId, setSelectedCategoryId] = useState<number | null>(null);
 
    const [submitting, setSubmitting] = useState(false);
    const [dialog, setDialog] = useState<DialogState>({
        open: false,
        level: "group",
        mode: "create",
        editingId: null,
        initialValues: emptyValues,
    });
 
    const { confirm, ConfirmDialogUI } = useConfirm();
 
    const loadGroups = useCallback(async (page: number) => {
        setGroupsLoading(true);
        try {
            const result = await getPagedGroups({page, pageSize: GROUP_PAGE_SIZE});
            setGroups(result.data.items);
            setGroupTotalPages(result.data.totalPages);
        } catch (err) {
            toast.error(err);
        } finally {
            setGroupsLoading(false);
        }
    }, []);
 
    const loadCategories = useCallback(async (groupId: number, page: number) => {
        setCategoriesLoading(true);
        try {
            const result = await getPagedCategories({groupId, page, pageSize: CATEGORY_PAGE_SIZE});
            setCategories(result.data.items);
            setCategoryTotalPages(result.data.totalPages);
        } catch (err) {
            toast.error(err);
        } finally {
            setCategoriesLoading(false);
        }
    }, []);
 
    const loadSubCategories = useCallback(async (categoryId: number, page: number) => {
        setSubCategoriesLoading(true);
        try {
            const result = await getPagedSubCategories({categoryId, page, pageSize: SUBCATEGORY_PAGE_SIZE});
            setSubCategories(result.data.items);
            setSubCategoryTotalPages(result.data.totalPages);
        } catch (err) {
            toast.error(err);
        } finally {
            setSubCategoriesLoading(false);
        }
    }, []);
 
    useEffect(() => {
        loadGroups(groupPage);
    }, [groupPage, loadGroups]);
 
    useEffect(() => {
        if (selectedGroupId == null) {
            setCategories([]);
            setCategoryTotalPages(1);
            setSelectedCategoryId(null);
            return;
        }
        setSelectedCategoryId(null);
        setSubCategories([]);
        setSubCategoryTotalPages(1);
        setCategoryPage(1);
        loadCategories(selectedGroupId, 1);
    }, [selectedGroupId, loadCategories]);
 
    useEffect(() => {
        if (selectedCategoryId == null) {
            setSubCategories([]);
            setSubCategoryTotalPages(1);
            return;
        }
        setSubCategoryPage(1);
        loadSubCategories(selectedCategoryId, 1);
    }, [selectedCategoryId, loadSubCategories]);
 
    function handleGroupPageChange(page: number) {
        setGroupPage(page);
    }
 
    function handleCategoryPageChange(page: number) {
        setCategoryPage(page);
        if (selectedGroupId != null) loadCategories(selectedGroupId, page);
    }
 
    function handleSubCategoryPageChange(page: number) {
        setSubCategoryPage(page);
        if (selectedCategoryId != null) loadSubCategories(selectedCategoryId, page);
    }
 
    function openCreateDialog(level: EntityLevel) {
        setDialog({ open: true, level, mode: "create", editingId: null, initialValues: emptyValues });
    }
 
    function openEditDialog(level: EntityLevel, item: { id: number; name: string }) {
        setDialog({
            open: true,
            level,
            mode: "edit",
            editingId: item.id,
            initialValues: { name: item.name },
        });
    }
 
    function closeDialog() {
        if (submitting) return;
        setDialog((prev) => ({ ...prev, open: false }));
    }
 
    async function handleDialogSubmit(values: EntityFormValues) {
        setSubmitting(true);
        try {
            if (dialog.level === "group") {
                if (dialog.mode === "create") {
                    const res = await createGroup(values);
                    toast.success(res.message);
                    setGroupPage(1);
                    await loadGroups(1);
                } else if (dialog.editingId != null) {
                    const res = await updateGroup(dialog.editingId, values);
                    toast.success(res.message);
                    await loadGroups(groupPage);
                }
            } else if (dialog.level === "category") {
                if (selectedGroupId == null) return;
                if (dialog.mode === "create") {
                    const res = await createCategory({ groupId: selectedGroupId, ...values });
                    toast.success(res.message);
                    setCategoryPage(1);
                    await loadCategories(selectedGroupId, 1);
                } else if (dialog.editingId != null) {
                    const res = await updateCategory(dialog.editingId, values);
                    toast.success(res.message);
                    await loadCategories(selectedGroupId, categoryPage);
                }
            } else {
                if (selectedCategoryId == null) return;
                if (dialog.mode === "create") {
                    const res = await createSubCategory({ categoryId: selectedCategoryId, ...values });
                    toast.success(res.message);
                    setSubCategoryPage(1);
                    await loadSubCategories(selectedCategoryId, 1);
                } else if (dialog.editingId != null) {
                    const res = await updateSubCategory(dialog.editingId, values);
                    toast.success(res.message);
                    await loadSubCategories(selectedCategoryId, subCategoryPage);
                }
            }
            setDialog((prev) => ({ ...prev, open: false }));
        } catch (err) {
            toast.error(err);
        } finally {
            setSubmitting(false);
        }
    }
 
    async function handleDeleteGroup(item: Group) {
        const result = await confirm({
            title: "Delete Group",
            message: `Are you sure you want to delete "${item.name}"? This will also remove its categories and subcategories.`,
            confirmText: "Delete",
        });
        if (!result.confirmed) return;
 
        try {
            const res = await deleteGroup(item.id);
            toast.success(res.message);
            if (selectedGroupId === item.id) setSelectedGroupId(null);
            setGroupPage(1);
            await loadGroups(1);
        } catch (err) {
            toast.error(err);
        }
    }
 
    async function handleDeleteCategory(item: Category) {
        const result = await confirm({
            title: "Delete Category",
            message: `Are you sure you want to delete "${item.name}"? This will also remove its subcategories.`,
            confirmText: "Delete",
        });
        if (!result.confirmed || selectedGroupId == null) return;
 
        try {
            const res = await deleteCategory(item.id);
            toast.success(res.message);
            if (selectedCategoryId === item.id) setSelectedCategoryId(null);
            setCategoryPage(1);
            await loadCategories(selectedGroupId, 1);
        } catch (err) {
            toast.error(err);
        }
    }
 
    async function handleDeleteSubCategory(item: SubCategory) {
        const result = await confirm({
            title: "Delete SubCategory",
            message: `Are you sure you want to delete "${item.name}"?`,
            confirmText: "Delete",
        });
        if (!result.confirmed || selectedCategoryId == null) return;
 
        try {
            const res = await deleteSubCategory(item.id);
            toast.success(res.message);
            setSubCategoryPage(1);
            await loadSubCategories(selectedCategoryId, 1);
        } catch (err) {
            toast.error(err);
        }
    }
 
    return (
        <>
            <Box sx={{ maxWidth: "100%" }}>
                <Typography variant="h5" sx={{ my: 2 }}>Group Management</Typography>
 
                <Stack direction="row" spacing={2} sx={{ width: "100%" }}>
                    <EntityColumn
                        title="Groups"
                        items={groups}
                        selectedId={selectedGroupId}
                        loading={groupsLoading}
                        page={groupPage}
                        totalPages={groupTotalPages}
                        onPageChange={handleGroupPageChange}
                        onSelect={setSelectedGroupId}
                        onAdd={() => openCreateDialog("group")}
                        onEdit={(item) => openEditDialog("group", item)}
                        onDelete={handleDeleteGroup}
                    />
 
                    <EntityColumn
                        title="Categories"
                        items={categories}
                        selectedId={selectedCategoryId}
                        loading={categoriesLoading}
                        disabled={selectedGroupId == null}
                        disabledMessage="Select a group to see its categories."
                        page={categoryPage}
                        totalPages={categoryTotalPages}
                        onPageChange={handleCategoryPageChange}
                        onSelect={setSelectedCategoryId}
                        onAdd={() => openCreateDialog("category")}
                        onEdit={(item) => openEditDialog("category", item)}
                        onDelete={handleDeleteCategory}
                    />
 
                    <EntityColumn
                        title="SubCategories"
                        items={subCategories}
                        selectedId={null}
                        loading={subCategoriesLoading}
                        disabled={selectedCategoryId == null}
                        disabledMessage="Select a category to see its subcategories."
                        page={subCategoryPage}
                        totalPages={subCategoryTotalPages}
                        onPageChange={handleSubCategoryPageChange}
                        onSelect={() => {}}
                        onAdd={() => openCreateDialog("subCategory")}
                        onEdit={(item) => openEditDialog("subCategory", item)}
                        onDelete={handleDeleteSubCategory}
                    />
                </Stack>
            </Box>
 
            <EntityFormDialog
                open={dialog.open}
                level={dialog.level}
                mode={dialog.mode}
                initialValues={dialog.initialValues}
                submitting={submitting}
                onClose={closeDialog}
                onSubmit={handleDialogSubmit}
            />
 
            {ConfirmDialogUI}
        </>
    );
}
 
export default GroupManagementPage;