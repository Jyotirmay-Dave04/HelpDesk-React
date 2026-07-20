import { Card, CardContent, Divider, Stack, Typography } from "@mui/material";
import type { Comment } from "../../interfaces/comment";
import CommentItem from "./CommentItem";
import CommentInput from "./CommentInput";

interface Props {
    comments: Comment[];
    onAddComment: (content: string, isInternal: boolean) => Promise<void>;
    canPostInternal: boolean;
  }
  
  const CommentsSection = ({ comments, onAddComment, canPostInternal }: Props) => {
    return (
      <Card variant="outlined">
        <CardContent>
          <Typography variant="subtitle1" gutterBottom sx={{fontWeight: 600}}>
            Comments ({comments.length})
          </Typography>
  
          <Stack spacing={2} sx={{ mb: 2 }}>
            {comments.length === 0 ? (
              <Typography variant="body2" color="text.secondary">
                No comments yet. Be the first to add one.
              </Typography>
            ) : (
              comments.map((c) => <CommentItem key={c.id} comment={c} />)
            )}
          </Stack>
  
          <Divider sx={{ mb: 2 }} />
  
          <CommentInput onSubmit={onAddComment} canPostInternal={canPostInternal} />
        </CardContent>
      </Card>
    );
  };
  
  export default CommentsSection;