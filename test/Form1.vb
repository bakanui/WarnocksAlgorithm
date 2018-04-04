Public Class Form1
    Dim graphics, graphics2 As Graphics
    Dim isClipping As Boolean
    Dim bitmap, bitmap2 As Bitmap
    Dim mRect As Rectangle
    Dim x, y As Integer
    Dim nS As Integer = 0
    Dim nIC As Integer = 0
    Dim w(3) As Rectangle

    ' Create pen.
    Dim blackPen As New Pen(Color.Black, 3)
    Dim backBrush As New SolidBrush(Color.White)

    ' Create points that define polygon.
    Dim point1 As New Point(50, 50)
    Dim point2 As New Point(100, 25)
    Dim point3 As New Point(200, 5)
    Dim point4 As New Point(250, 50)
    Dim point5 As New Point(300, 100)
    Dim point6 As New Point(350, 200)
    Dim point7 As New Point(250, 250)
    Dim curvePoints As Point() = {point1, point4, point6}

    Dim blueBrush As New SolidBrush(Color.Blue)

    Dim easy As Boolean

    Structure TRect

    End Structure


    Private Sub ButtonClip_Click(sender As Object, e As EventArgs) Handles ButtonClip.Click

        mRect.X = x
        mRect.Y = y


        If x <= Canvas1.Width - 1 And x > 0 Then
            If x > mRect.X Then
                mRect.Width = x - mRect.X
            ElseIf x < mRect.X Then
                mRect.Width = mRect.X - x
                mRect.X = x
            End If
        End If
        If y <= Canvas1.Height - 1 And y > 0 Then
            If y > mRect.Y Then
                mRect.Height = y - mRect.Y
            ElseIf y < mRect.Y Then
                mRect.Height = mRect.Y - y
                mRect.Y = x
                graphics2.DrawRectangle(New Pen(Color.Black, 1), mRect)
            End If
        End If
        Canvas1.Image = bitmap2
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        bitmap = New Bitmap(Canvas1.Width, Canvas1.Height)
        graphics = Graphics.FromImage(bitmap)
        Canvas1.Image = bitmap
        InitWindow(Canvas1.Width, Canvas1.Height)

        ' graphics.FillPolygon(blueBrush, curvePoints)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        graphics.FillPolygon(blueBrush, curvePoints)
    End Sub

    Private Sub Warnock(w As Rectangle)
        'surround intersect contain disjoint ?
        easy = False
        If nS = 0 And nIC = 0 Then
            graphics.FillRectangle(backBrush, w)
            easy = True
        ElseIf nS = 1 And nIC = 0 Then
            graphics.FillPolygon(blueBrush, curvePoints)
            easy = True
        ElseIf nS >= 1 Then
            easy = True
        End If
        If Not easy Then

        End If
    End Sub

    Private Sub PolygonClipping(Poly As Point())

    End Sub

    Private Sub InitWindow(width As Integer, height As Integer)
        w(0) = New Rectangle(0, 0, width / 2, height / 2)
        w(1) = New Rectangle(width / 2, 0, width / 2, height / 2)
        w(2) = New Rectangle(0, height / 2, width / 2, height / 2)
        w(3) = New Rectangle(width / 2, height / 2, width / 2, height / 2)
    End Sub


End Class
