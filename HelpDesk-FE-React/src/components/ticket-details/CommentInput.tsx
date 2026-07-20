import { Box, Button, Checkbox, FormControlLabel, Stack, TextField } from '@mui/material';
import { useFormik } from 'formik';
import * as Yup from 'yup';
import { toast } from '../../utils/Toast';

interface Props {
    onSubmit: (content: string, isInternal: boolean) => Promise<void>;
    canPostInternal: boolean;
}

const validationSchema = Yup.object({
    content: Yup.string().trim().required('Comment cannot be empty').max(2000, 'Max 2000 characters').min(5, 'Min 5 character required'),
    isInternal: Yup.boolean(),
});

const CommentInput = ({ onSubmit, canPostInternal }: Props) => {

    const formik = useFormik({
        initialValues: { content: '', isInternal: false },
        validationSchema,
        onSubmit: async (values, { resetForm, setSubmitting }) => {
            try {
                await onSubmit(values.content.trim(), values.isInternal);
                resetForm();
            } catch (error){
                toast.error(error);
            } finally {
                setSubmitting(false);
            }
        },
    });

    return (
        <Box component="form" onSubmit={formik.handleSubmit}>
            <TextField
                fullWidth
                multiline
                minRows={3}
                placeholder="Write a comment..."
                name="content"
                value={formik.values.content}
                onChange={formik.handleChange}
                onBlur={formik.handleBlur}
                error={formik.touched.content && Boolean(formik.errors.content)}
                helperText={formik.touched.content && formik.errors.content}
            />
            <Stack direction="row" sx={{ mt: 1, justifyContent: 'space-between', alignItems: 'center' }}>
                {canPostInternal ? (
                    <FormControlLabel
                        control={
                            <Checkbox
                                name="isInternal"
                                checked={formik.values.isInternal}
                                onChange={formik.handleChange}
                            />
                        }
                        label="Internal note (not visible to requester)"
                    />
                ) : (
                    <Box />
                )}
                <Button type="submit" variant="contained" disabled={formik.isSubmitting}>
                    Add Comment
                </Button>
            </Stack>
        </Box>
    );
};

export default CommentInput;