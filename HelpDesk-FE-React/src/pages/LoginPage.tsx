import { useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { useFormik } from "formik";
import * as Yup from "yup";
import { Alert, Box, Button, IconButton, InputAdornment, Stack, TextField, Typography } from "@mui/material";
import { Visibility, VisibilityOff } from "@mui/icons-material";
import { toast } from "../utils/Toast";
import { useAppDispatch, useAppSelector } from "../app/hooks/hooks";
import { login } from "../app/thunks/auth-thunk";

const validationSchema = Yup.object({
    email: Yup.string().required("Email is required").email("Email is invalid"),
    password: Yup.string().required("Password is required")
        .min(8, "Password must have at least 8 characters")
        .matches(/^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[^\w\s]).{8,}$/,
            "Password must contain at least 8 characters, one uppercase letter, one lowercase letter, one number, and one special character")
});

export default function LoginForm() {
    const dispatch = useAppDispatch();
    const { error } = useAppSelector(state => state.auth);
    const [showPassword, setShowPassword] = useState(false);
    const navigate = useNavigate();
    const location = useLocation();
    const from = location.state?.from?.pathname || '/dashboard';

    const formik = useFormik({
        initialValues: {
            email: '',
            password: ''
        },
        validationSchema,
        onSubmit: async (values, { setSubmitting }) => {
            try {
                const data = await dispatch(login(values)).unwrap();
                toast.success(data.message);
                navigate(from, { replace: true });
            } catch (err) {
                toast.error(error);
            } finally {
                setSubmitting(false);
            }
        }
    });

    return (
        <>
            <Box component="form" onSubmit={formik.handleSubmit} sx={{ maxWidth: 400, mx: 'auto', mt: 4 }}>
                <Stack direction="column" spacing={3}>
                    <Typography variant="h5" sx={{ mb: 1 }}>
                        Login
                    </Typography>

                    {error && (
                        <Alert severity="error" sx={{ mb: 2 }}>
                            {error}
                        </Alert>
                    )}

                    <TextField
                        fullWidth
                        margin="normal"
                        label="Email"
                        name="email"
                        type='email'
                        autoFocus
                        value={formik.values.email}
                        onChange={formik.handleChange}
                        onBlur={formik.handleBlur}
                        error={formik.touched.email && !!formik.errors.email}
                        helperText={formik.touched.email && formik.errors.email}
                    />

                    <TextField
                        fullWidth
                        margin="normal"
                        label="Password"
                        name="password"
                        type={showPassword ? 'text' : 'password'}
                        value={formik.values.password}
                        onChange={formik.handleChange}
                        onBlur={formik.handleBlur}
                        error={formik.touched.password && !!formik.errors.password}
                        helperText={formik.touched.password && formik.errors.password}
                        slotProps={{
                            input: {
                                endAdornment: (
                                    <InputAdornment position="end">
                                        <IconButton edge='end' onClick={() => setShowPassword(prev => !prev)}>
                                            {showPassword ? <VisibilityOff /> : <Visibility />}
                                        </IconButton>
                                    </InputAdornment>
                                )
                            }
                        }}
                    />

                    <Button
                        fullWidth
                        type='submit'
                        variant='contained'
                        sx={{ mt: 2 }}
                        disabled={formik.isSubmitting}
                        loading={formik.isSubmitting}
                    >
                        Login
                    </Button>
                </Stack>
            </Box>
        </>
    );
}