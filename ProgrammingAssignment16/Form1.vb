Public Class Form1
    Dim graphics As Graphics
    Dim canvas As Bitmap
    Dim vertices(7) As Vertex
    Dim edges(12) As Edge
    Dim vpolygon As List(Of Vertex)
    Dim epolygon As List(Of Edge)



    Dim view(3, 3), screen(3, 3) As Single
    Dim VR(7), VS(7) As Point
    Dim deg As Single = 0
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        canvas = New Bitmap(PictureBox1.Width, PictureBox1.Height)
        graphics = Graphics.FromImage(canvas)
    End Sub
    Structure Edge
        Dim point1, point2 As Integer
    End Structure
    Structure Vertex
        Dim x, y, z, w As Single
    End Structure
    Structure Mesh
        Dim e1, e2, e3, e4 As Edge
    End Structure
    Sub SetColMat(ByRef Matrix(,) As Single, col As Integer, a As Double, b As Double, c As Double, d As Double)
        Matrix(0, col) = a
        Matrix(1, col) = b
        Matrix(2, col) = c
        Matrix(3, col) = d
    End Sub
    Function MultiplyMat(vertex As Vertex, M(,) As Single) As Vertex
        Dim result As Vertex
        result.x = (vertex.x * M(0, 0) + vertex.y * M(1, 0) + vertex.z * M(2, 0) + vertex.w * M(3, 0))
        result.y = (vertex.x * M(0, 1) + vertex.y * M(1, 1) + vertex.z * M(2, 1) + vertex.w * M(3, 1))
        result.z = (vertex.x * M(0, 2) + vertex.y * M(1, 2) + vertex.z * M(2, 2) + vertex.w * M(3, 2))
        result.w = 1
        Return result
    End Function
    Function DegreeToRadian(ByRef degree As Integer)
        Return degree * Math.PI / 180
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

    End Sub

    Sub SetEdge(ByRef edge As Edge, n1 As Integer, n2 As Integer)
        edge.point1 = n1
        edge.point2 = n2
    End Sub
    Sub SetVertex(ByRef Vertex As Vertex, x As Integer, y As Integer, z As Integer)
        x = Integer.Parse(TextBoxX.Text)
        y = Integer.Parse(TextBoxY.Text)
        z = Integer.Parse(TextBoxZ.Text)

        Vertex.x = x
        Vertex.y = y
        Vertex.z = z
        Vertex.w = 1
    End Sub


End Class
