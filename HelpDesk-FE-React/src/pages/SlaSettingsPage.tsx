import * as Yup from "yup";
import { useEffect, useState } from "react";
import type { SlaPolicyResponse, UpdateSlaPolicyPayload } from "../interfaces/sla-policy";
import { getAllSlaPolicies, updateSlaPolicy } from "../services/sla-policy-service";
import { toast } from "../utils/Toast";
import { Box, Button, CircularProgress, Paper, Stack, TextField, Typography } from "@mui/material";
import { Form, Formik, type FormikErrors } from "formik";


const MIN_SLA_HOURS = 1;
const MAX_SLA_HOURS = 720;

function SlaSettingsPage() {
    const [policies, setPolicies] = useState<SlaPolicyResponse[]>([]);
    const [loading, setLoading] = useState(false);
    const [submitting, setSubmitting] = useState(false);

    useEffect(() => { loadPolicies(); }, []);

    async function loadPolicies() {
        setLoading(true);
        try {
            const data = await getAllSlaPolicies();
            setPolicies(data.data);
        } catch (err) {
            toast.error(err);
        } finally {
            setLoading(false);
        }
    }

    async function handleSubmit(values: { policies: SlaPolicyResponse[] }) {
        setSubmitting(true);
        try {
            const updates = values.policies
                .filter((p) => policies.find((op) => op.priority === p.priority)?.hoursToResolve !== p.hoursToResolve)
                .map((p) => updateSlaPolicy(p.priority, { hoursToResolve: p.hoursToResolve } as UpdateSlaPolicyPayload));

            await Promise.all(updates);
            toast.success("SLA policy updated");
            await loadPolicies();
        } catch (err) {
            toast.error(err);
        } finally {
            setSubmitting(false);
        }
    }

    if (loading) {
        return <Stack sx={{ height: "50vh", alignItems: 'center', justifyContent: 'center' }}><CircularProgress /></Stack>;
    }

    return (
        <Box sx={{ maxWidth: 520 }}>
            <Typography variant="h5" sx={{ my: 2 }}>SLA Settings</Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                Set the SLA deadline, in whole hours ({MIN_SLA_HOURS}–{MAX_SLA_HOURS}), for each priority.
            </Typography>

            <Paper variant="outlined" sx={{ p: 3 }}>
                <Formik
                    enableReinitialize
                    initialValues={{ policies }}
                    validationSchema={Yup.object({
                        policies: Yup.array().of(
                            Yup.object({
                                hoursToResolve: Yup.number()
                                    .typeError("Must be a number")
                                    .integer("Whole hours only")
                                    .min(MIN_SLA_HOURS, `Minimum is ${MIN_SLA_HOURS} hour`)
                                    .max(MAX_SLA_HOURS, `Maximum is ${MAX_SLA_HOURS} hours`)
                                    .required("Required"),
                            })
                        ),
                    })}
                    onSubmit={handleSubmit}
                >
                    {({ values, errors, touched, handleChange, handleBlur, handleSubmit: submitForm, dirty }) => (
                        <Form onSubmit={submitForm} noValidate>
                            <Stack spacing={2}>
                                {values.policies.map((policy, index) => (
                                    <TextField
                                        key={policy.priority}
                                        fullWidth
                                        type="number"
                                        name={`policies[${index}].hoursToResolve`}
                                        label={`${policy.priority} priority (hours)`}
                                        value={policy.hoursToResolve}
                                        onChange={handleChange}
                                        onBlur={handleBlur}
                                        error={!!touched.policies?.[index]?.hoursToResolve && !!(errors.policies?.[index] as FormikErrors<SlaPolicyResponse>)?.hoursToResolve}
                                        helperText={
                                            touched.policies?.[index]?.hoursToResolve
                                                ? (errors.policies?.[index] as FormikErrors<SlaPolicyResponse>)?.hoursToResolve
                                                : ""
                                        }
                                        slotProps={{ input: { inputProps: { min: MIN_SLA_HOURS, max: MAX_SLA_HOURS, step: 1 } } }}
                                    />
                                ))}
                                <Box>
                                    <Button type="submit" variant="contained" disabled={submitting || !dirty}>
                                        Save Changes
                                    </Button>
                                </Box>
                            </Stack>
                        </Form>
                    )}
                </Formik>
            </Paper>
        </Box>
    );
}

export default SlaSettingsPage;