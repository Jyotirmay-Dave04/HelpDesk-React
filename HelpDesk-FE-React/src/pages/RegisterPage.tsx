import * as Yup from 'yup';
import { useAppDispatch, useAppSelector } from '../app/hooks/hooks';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useFormik } from 'formik';
import { Alert, Box, Button, IconButton, InputAdornment, Link, Stack, TextField, Typography } from '@mui/material';
import { Visibility, VisibilityOff } from '@mui/icons-material';
import { register } from '../app/thunks/auth-thunk';
import { toast } from '../utils/Toast';

const validationSchema = Yup.object({
    name: Yup.string()
        .required("Name is required")
        .min(2, "Name must have at least 2 characters")
        .max(100, "Name cannot exceed 100 characters")
        .matches(/^[A-Za-z_][A-Za-z0-9_ ]*$/, "Name must start with a letter or underscore and contain no special characters"),
    email: Yup.string().required("Email is required").email("Email must be valid"),
    password: Yup.string().required("Password is required")
        .min(8, "Password must have at least 8 characters")
        .matches(/^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[^\w\s]).{8,}$/,
            "Password must contain at least 8 characters, one uppercase letter, one lowercase letter, one number, and one special character")
});

function RegisterPage() {
    const dispatch = useAppDispatch();
    const { error } = useAppSelector(state => state.auth);
    const [showPassword, setShowPassword] = useState(false);
    const navigate = useNavigate();

    const formik = useFormik({
        initialValues: {
            name: '',
            email: '',
            password: ''
        },
        validationSchema,
        onSubmit: async (values, { setSubmitting }) => {
            setSubmitting(true);
            try {
                const response = await dispatch(register(values)).unwrap();
                toast.success(response.message);
                setTimeout(() => {
                    navigate('/dashboard');
                }, 500);
            } catch (error) {
                toast.error(error);
            } finally {
                setSubmitting(false);
            }
        }
    })

    return (
        <Box component="form" onSubmit={formik.handleSubmit} sx={{ maxWidth: 400, mx: 'auto', mt: 4 }} noValidate>
            <Stack direction="column" spacing={1}>
                <Typography variant="h5" sx={{ mb: 1 }}>
                    Register
                </Typography>

                {error && (
                    <Alert severity="error" sx={{ mb: 2 }}>
                        {error}
                    </Alert>
                )}

                <TextField 
                    fullWidth
                    margin='normal'
                    name='name'
                    label="Name"
                    type='text'
                    autoFocus
                    value={formik.values.name}
                    onChange={formik.handleChange}
                    onBlur={formik.handleBlur}
                    error={formik.touched.name && !!formik.errors.name}
                    helperText={(formik.touched.name && formik.errors.name) || ' '}
                />

                <TextField
                    fullWidth
                    margin="normal"
                    label="Email"
                    name="email"
                    type='email'
                    value={formik.values.email}
                    onChange={formik.handleChange}
                    onBlur={formik.handleBlur}
                    error={formik.touched.email && !!formik.errors.email}
                    helperText={(formik.touched.email && formik.errors.email) || ' '}
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
                    helperText={(formik.touched.password && formik.errors.password) || ' '}
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
                    Create Account
                </Button>

                <Typography variant='subtitle1' sx={{ justifyContent: 'flex-end'}}>
                        Already have an account? <Link href='/login'>Sign In</Link>
                    </Typography>
            </Stack>
        </Box>
    );
}

export default RegisterPage;