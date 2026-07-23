import { Dialog, DialogTitle, DialogContent, TextField, DialogActions, Button } from "@mui/material";
import { Form, Formik } from "formik";
import type { EntityFormValues, EntityLevel } from "../../interfaces/groups";
import * as Yup from 'yup';

const labels: Record<EntityLevel, string> = {
    group: "Group",
    category: "Category",
    subCategory: "SubCategory",
};
 
const validationSchema = Yup.object({
    name: Yup.string().trim().required("Name is required").max(100, "Name must be 100 characters or fewer"),
});
 
interface EntityFormDialogProps {
    open: boolean;
    level: EntityLevel;
    mode: "create" | "edit";
    initialValues: EntityFormValues;
    submitting: boolean;
    onClose: () => void;
    onSubmit: (values: EntityFormValues) => void;
}
 
function EntityFormDialog({ open, level, mode, initialValues, submitting, onClose, onSubmit }: EntityFormDialogProps) {
    const title = `${mode === "create" ? "Add" : "Edit"} ${labels[level]}`;
 
    return (
        <Dialog open={open} onClose={onClose} fullWidth maxWidth="xs">
            <Formik
                enableReinitialize
                initialValues={initialValues}
                validationSchema={validationSchema}
                onSubmit={onSubmit}
            >
                {({ values, errors, touched, handleChange, handleBlur, handleSubmit }) => (
                    <Form onSubmit={handleSubmit}>
                        <DialogTitle>{title}</DialogTitle>
                        <DialogContent sx={{ pt: 1 }}>
                            <TextField
                                autoFocus
                                fullWidth
                                name="name"
                                label="Name"
                                value={values.name}
                                onChange={handleChange}
                                onBlur={handleBlur}
                                error={touched.name && !!errors.name}
                                helperText={touched.name && errors.name}
                            />
                        </DialogContent>
                        <DialogActions sx={{ px: 3, pb: 2 }}>
                            <Button onClick={onClose} disabled={submitting}>Cancel</Button>
                            <Button type="submit" variant="contained" disabled={submitting || values.name === initialValues.name}>
                                {mode === "create" ? "Create" : "Save"}
                            </Button>
                        </DialogActions>
                    </Form>
                )}
            </Formik>
        </Dialog>
    );
}
 
export default EntityFormDialog;