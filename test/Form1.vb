Public Class Form1
    Dim graphics, graphics2 As Graphics
    Dim isClipping As Boolean
    Dim bitmap, bitmap2 As Bitmap
    Dim mRect As Rectangle
    Dim x, y As Integer

    Private Sub ButtonClip_Click(sender As Object, e As EventArgs) Handles ButtonClip.Click

        mRect.X = x
            mRect.Y = y


        If x <= PictureBox1.Width - 1 And x > 0 Then
            If x > mRect.X Then
                mRect.Width = x - mRect.X
            ElseIf x < mRect.X Then
                mRect.Width = mRect.X - x
                mRect.X = x
            End If
        End If
        If y <= PictureBox1.Height - 1 And e.Y > 0 Then
            If y > mRect.Y Then
                mRect.Height = y - mRect.Y
            ElseIf y < mRect.Y Then
                mRect.Height = mRect.Y - y
                mRect.Y = x
                graphics2.DrawRectangle(New Pen(Color.Black, 1), mRect)
            End If
        End If
        PictureBox1.Image = bitmap2
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

End Class
