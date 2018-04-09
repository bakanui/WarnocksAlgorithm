Public Class Form1
    Dim graphics As Graphics
    Dim canvas As Bitmap
    Dim vertex1(3), vertex2(4) As Vertex
    Dim edge1(5), edge2(8) As Edge
    Dim mesh1(3), mesh2(5) As Mesh
    Dim pr(3, 3) As Single
    Dim Translate(3, 3), Rotatex(3, 3), Rotatey(3, 3), Rotatez(3, 3) As Single
    Dim view(3, 3), screen(3, 3) As Single
    Dim dx, dy, dz, tetax, tetay, tetaz As Single
    Dim dx2, dy2, dz2, tetax2, tetay2, tetaz2 As Single
    Dim firstrun As Boolean
    Dim VS1(4), VS2(4) As Vertex
    Public Structure PolygonVertexIndex
        Public v1, v2, v3 As Byte
        Public color As Brush
        Public Sub New(_v1 As Byte, _v2 As Byte, _v3 As Byte, _color As Brush)
            v1 = _v1
            v2 = _v2
            v3 = _v3
            color = _color
        End Sub
    End Structure
    Public Structure PolygonVertex
        Public vertex As List(Of Vertex)
        Public color As Brush
        Public Sub New(_vertex As List(Of Vertex), _color As Brush)
            vertex = New List(Of Vertex)
            vertex.AddRange(_vertex)
            color = _color
        End Sub
        Public Function Find_Minimum() As Integer
            Dim zMin = vertex.Last.z
            For Each v In vertex
                If zMin > v.z Then
                    zMin = v.z
                End If
            Next
            Return zMin
        End Function
        Public Function Find_Maximum() As Integer
            Dim zMax = vertex.Last.z
            For Each v In vertex
                If zMax < v.z Then
                    zMax = v.z
                End If
            Next
            Return zMax
        End Function
    End Structure
    Public Structure EdgeWindow
        Public x1, y1, x2, y2 As Short
        Public Sub New(v1 As Point, v2 As Point)
            x1 = v1.X
            y1 = v1.Y
            x2 = v2.X
            y2 = v2.Y
        End Sub
    End Structure
    Structure Vector
        Public i, j, k As Single
    End Structure
    Structure Edge
        Dim point1, point2 As Integer
    End Structure
    Structure Vertex
        Dim x, y, z, w As Single
        Public Sub New(_x As Single, _y As Single, _z As Single)
            x = _x
            y = _y
            z = _z
            w = 1
        End Sub
        Public Function ToShort() As Vertex
            Return New Vertex(Convert.ToInt16(x), Convert.ToInt16(y), Convert.ToInt16(z))
        End Function
    End Structure
    Structure Mesh
        Dim vertex1, vertex2, vertex3 As Integer
        Dim isVisible As Boolean
    End Structure
    Sub SetVector(ByRef vector As Vector, x As Single, y As Single, z As Single)
        vector.i = x
        vector.j = y
        vector.k = z
    End Sub
    Sub SetMesh(ByRef mesh As Mesh, ByVal m1 As Integer, ByVal m2 As Integer, ByVal m3 As Integer)
        mesh.vertex1 = m1
        mesh.vertex2 = m2
        mesh.vertex3 = m3
    End Sub
    Sub SetColMat(ByRef Matrix(,) As Single, col As Integer, a As Double, b As Double, c As Double, d As Double)
        Matrix(0, col) = a
        Matrix(1, col) = b
        Matrix(2, col) = c
        Matrix(3, col) = d
    End Sub
    Sub SetEdge(ByRef edge As Edge, n1 As Integer, n2 As Integer)
        edge.point1 = n1
        edge.point2 = n2
    End Sub
    Sub SetVertex(ByRef Vertex As Vertex, x As Single, y As Single, z As Single)
        Vertex.x = x
        Vertex.y = y
        Vertex.z = z
        Vertex.w = 1
    End Sub
    Function MultiplyMat1(point As Vertex, M(,) As Single) As Vertex
        Dim result As Vertex

        result.x = (point.x * M(0, 0) + point.y * M(1, 0) + point.z * M(2, 0) + point.w * M(3, 0))
        result.y = (point.x * M(0, 1) + point.y * M(1, 1) + point.z * M(2, 1) + point.w * M(3, 1))
        result.z = (point.x * M(0, 2) + point.y * M(1, 2) + point.z * M(2, 2) + point.w * M(3, 2))
        result.w = (point.x * M(0, 3) + point.y * M(1, 3) + point.z * M(2, 3) + point.w * M(3, 3))

        result.x = result.x / result.w
        result.y = result.y / result.w
        result.z = result.z / result.w
        result.w = 1

        Return result
    End Function
    Function MultiplyMat2(M1(,) As Single, M2(,) As Single) As Single(,)
        Dim result(3, 3) As Single

        For row = 0 To 3
            For col = 0 To 3
                result(row, col) = (M1(row, 0) * M2(0, col)) + (M1(row, 1) * M2(1, col)) + (M1(row, 2) * M2(2, col)) + (M1(row, 3) * M2(3, col))
            Next
        Next

        Return result
    End Function
    'functions to calculate cos
    Function cos(x As Single) As Single
        Return Math.Cos(x * (Math.PI / 180))
    End Function
    'functions to calculate sin
    Function sin(x As Single) As Single
        Return Math.Sin(x * (Math.PI / 180))
    End Function
    'functions to calculate dot product
    Function DotProductof(V1 As Vector, V2 As Vector) As Single
        Return (V1.i * V2.i) + (V1.j * V2.j) + (V1.k * V2.k)
    End Function
    'functions to calculate cross product
    Function CrossProductof(V1 As Vector, V2 As Vector) As Vector
        Dim result As New Vector
        result.i = (V1.j * V2.k) - (V1.k * V2.j)
        result.j = (V1.k * V2.i) - (V1.i * V2.k)
        result.k = (V1.i * V2.j) - (V1.j * V2.i)
        Return result
    End Function
    'set translate and rotation matrixes
    Sub SetTranslateRotateMatrix(ByRef dx As Single, ByRef dy As Single, ByRef dz As Single, ByRef tetax As Single, ByRef tetay As Single, ByRef tetaz As Single)
        SetColMat(Translate, 0, 1, 0, 0, dx)
        SetColMat(Translate, 1, 0, 1, 0, dy)
        SetColMat(Translate, 2, 0, 0, 1, dz)
        SetColMat(Translate, 3, 0, 0, 0, 1)

        SetColMat(Rotatez, 0, cos(tetaz), -sin(tetaz), 0, 0)
        SetColMat(Rotatez, 1, sin(tetaz), cos(tetaz), 0, 0)
        SetColMat(Rotatez, 2, 0, 0, 1, 0)
        SetColMat(Rotatez, 3, 0, 0, 0, 1)


        SetColMat(Rotatey, 0, cos(tetay), 0, sin(tetay), 0)
        SetColMat(Rotatey, 1, 0, 1, 0, 0)
        SetColMat(Rotatey, 2, -sin(tetay), 0, cos(tetay), 0)
        SetColMat(Rotatey, 3, 0, 0, 0, 1)


        SetColMat(Rotatex, 0, 1, 0, 0, 0)
        SetColMat(Rotatex, 1, 0, cos(tetax), -sin(tetax), 0)
        SetColMat(Rotatex, 2, 0, sin(tetax), cos(tetax), 0)
        SetColMat(Rotatex, 3, 0, 0, 0, 1)
    End Sub
    'set the vertices
    Sub SetDefaultPoints()
        For i As Integer = 0 To 3
            vertex1(i) = New Vertex
            vertex2(i) = New Vertex
        Next
        vertex2(4) = New Vertex

        SetVertex(vertex1(0), -1, -1, -1)
        SetVertex(vertex1(1), 1, -1, -1)
        SetVertex(vertex1(2), 0, -1, 1)
        SetVertex(vertex1(3), 0, 2, 0)

        SetVertex(vertex2(0), -1, -1, -1)
        SetVertex(vertex2(1), 1, -1, -1)
        SetVertex(vertex2(2), 1, -1, 1)
        SetVertex(vertex2(3), -1, -1, 1)
        SetVertex(vertex2(4), 0, 2, 0)
    End Sub
    'set the edges with vertex index
    Sub SetDefaultEdges()
        For i As Integer = 0 To 5
            edge1(i) = New Edge
            edge2(i) = New Edge
        Next
        edge2(6) = New Edge
        edge2(7) = New Edge
        edge2(8) = New Edge

        SetEdge(edge1(0), 0, 1)
        SetEdge(edge1(1), 0, 3)
        SetEdge(edge1(2), 0, 2)
        SetEdge(edge1(3), 1, 2)
        SetEdge(edge1(4), 1, 3)
        SetEdge(edge1(5), 2, 3)

        SetEdge(edge2(0), 0, 1)
        SetEdge(edge2(1), 1, 2)
        SetEdge(edge2(2), 2, 3)
        SetEdge(edge2(3), 3, 0)
        SetEdge(edge2(4), 0, 4)
        SetEdge(edge2(5), 1, 4)
        SetEdge(edge2(6), 2, 4)
        SetEdge(edge2(7), 3, 4)
        SetEdge(edge2(8), 0, 2)
    End Sub
    'set the meshes with vertex index
    Sub SetDefaultMeshes()
        For i As Integer = 0 To 3
            mesh1(i) = New Mesh
            mesh2(i) = New Mesh
        Next
        mesh2(4) = New Mesh
        mesh2(5) = New Mesh

        'Base on Vertex
        SetMesh(mesh1(0), 0, 1, 2)
        SetMesh(mesh1(1), 0, 2, 3)
        SetMesh(mesh1(2), 1, 2, 3)
        SetMesh(mesh1(3), 0, 1, 3)

        'Base on Vertex
        SetMesh(mesh2(0), 0, 1, 4)
        SetMesh(mesh2(1), 1, 2, 4)
        SetMesh(mesh2(2), 2, 3, 4)
        SetMesh(mesh2(3), 3, 0, 4)
        SetMesh(mesh2(4), 0, 1, 2)
        SetMesh(mesh2(5), 2, 3, 0)
    End Sub
    'calculates backface culling of the polymesh (also no clue)
    Sub BackFaceCulling(ByRef mesh1() As Mesh, ByRef mesh2() As Mesh, coor1() As Vertex, coor2() As Vertex)
        Dim N, V As New Vector

        SetVector(V, 0, 0, -4)

        For i = 0 To 3
            N = Normalof(mesh1(i), coor1)
            If DotProductof(V, N) >= 0 Then
                mesh1(i).isVisible = False
            Else
                mesh1(i).isVisible = True
            End If
        Next

        For j = 0 To 4
            N = Normalof(mesh2(j), coor2)
            If DotProductof(V, N) >= 0 Then
                mesh2(j).isVisible = False
            Else
                mesh2(j).isVisible = True
            End If
        Next

    End Sub
    'calculated normal of polymesh (no idea what it does tho)
    Function Normalof(mesh As Mesh, Point() As Vertex) As Vector
        Dim A, B As Vector
        Dim p1, p2, p3 As Integer

        p1 = mesh.vertex1
        p2 = mesh.vertex2
        p3 = mesh.vertex3

        A = New Vector
        B = New Vector

        A.i = Point(p2).x - Point(p1).x
        A.j = Point(p2).y - Point(p1).y
        A.k = Point(p2).z - Point(p1).z
        SetVector(A, A.i, A.j, A.k)
        B.i = Point(p3).x - Point(p1).x
        B.j = Point(p3).y - Point(p1).y
        B.k = Point(p3).z - Point(p1).z
        SetVector(B, B.i, B.j, B.k)

        Return CrossProductof(A, B)
    End Function
    Sub WarnockAlgorithm(pbc() As Mesh)
        'TODO warnock's algorithm here
    End Sub
    Sub PolygonClipping(mesh() As Mesh) 'accepts poly, rect, c (?)

    End Sub
    'handles the drawing of the two pyramids
    Sub DrawPyramids()
        graphics.Clear(PictureBox1.BackColor)
        CalculatePyramid1()
        CalculatePyramid2()
        BackFaceCulling(mesh1, mesh2, VS1, VS2)
        Warnock(0, 0, 2 ^ 8, 2 ^ 8, 9)
        Warnock(2 ^ 8, 0, 2 ^ 9, 2 ^ 8, 9)
        Warnock(0, 2 ^ 8, 2 ^ 8, 2 ^ 9, 9)
        Warnock(2 ^ 8, 2 ^ 8, 2 ^ 9, 2 ^ 9, 9)
        DrawLinesPyramid1(Color.Black)
        DrawLinesPyramid2(Color.Black)
        PictureBox1.Refresh()
    End Sub
    'calculates pyramid 1
    Sub CalculatePyramid1()
        Dim VR1(4) As Vertex

        SetTranslateRotateMatrix(dx, dy, dz, tetax, tetay, tetaz)

        For i = 0 To 3
            pr = MultiplyMat2(Rotatez, Rotatey)
            pr = MultiplyMat2(pr, Rotatex)
            pr = MultiplyMat2(pr, Translate)
            VR1(i) = MultiplyMat1(vertex1(i), pr)
            VS1(i) = MultiplyMat1(VR1(i), view)
            VS1(i) = MultiplyMat1(VS1(i), screen)
        Next
    End Sub
    'calculates pyramid 2
    Sub CalculatePyramid2()
        Dim VR2(4) As Vertex
        SetTranslateRotateMatrix(dx2, dy2, dz2, tetax2, tetay2, tetaz2)

        For i = 0 To 4
            pr = MultiplyMat2(Rotatez, Rotatey)
            pr = MultiplyMat2(pr, Rotatex)
            pr = MultiplyMat2(pr, Translate)
            VR2(i) = MultiplyMat1(vertex2(i), pr)
            VS2(i) = MultiplyMat1(VR2(i), view)
            VS2(i) = MultiplyMat1(VS2(i), screen)
        Next
    End Sub
    Sub DrawLinesPyramid1(color As Color)
        Dim pen = New Pen(Color.Black)
        Dim a, b, c, d, e, f, g, h, j, k, l, m As Integer
        For i = 0 To 3
            a = VS1(mesh1(i).vertex1).x
            b = VS1(mesh1(i).vertex1).y
            c = VS1(mesh1(i).vertex2).x
            d = VS1(mesh1(i).vertex2).y
            graphics.DrawLine(pen, a, b, c, d)
            e = VS1(mesh1(i).vertex2).x
            f = VS1(mesh1(i).vertex2).y
            g = VS1(mesh1(i).vertex3).x
            h = VS1(mesh1(i).vertex3).y
            graphics.DrawLine(pen, e, f, g, h)
            j = VS1(mesh1(i).vertex3).x
            k = VS1(mesh1(i).vertex3).y
            l = VS1(mesh1(i).vertex1).x
            m = VS1(mesh1(i).vertex1).y
            graphics.DrawLine(pen, j, k, l, m)
        Next
    End Sub
    Sub DrawLinesPyramid2(color As Color)
        Dim pen = New Pen(color)
        Dim a, b, c, d, e, f, g, h, j, k, l, m As Integer
        For i = 0 To 4
            a = VS2(mesh2(i).vertex1).x
            b = VS2(mesh2(i).vertex1).y
            c = VS2(mesh2(i).vertex2).x
            d = VS2(mesh2(i).vertex2).y
            graphics.DrawLine(pen, a, b, c, d)
            e = VS2(mesh2(i).vertex2).x
            f = VS2(mesh2(i).vertex2).y
            g = VS2(mesh2(i).vertex3).x
            h = VS2(mesh2(i).vertex3).y
            graphics.DrawLine(pen, e, f, g, h)
            j = VS2(mesh2(i).vertex3).x
            k = VS2(mesh2(i).vertex3).y
            l = VS2(mesh2(i).vertex1).x
            m = VS2(mesh2(i).vertex1).y
            graphics.DrawLine(pen, j, k, l, m)
        Next
    End Sub
    'initializes view and screen matrix, canvas, graphics, and variables used for translating and rotating
    Private Sub Form1_Load(sender As Object, eA As EventArgs) Handles MyBase.Load
        Me.KeyPreview = True
        firstrun = True
        tetaz = 0
        tetay = 0
        tetax = 0
        dx = -2.5
        dy = 0
        dz = 0
        dx2 = 2
        dy2 = 0
        dz2 = 0
        canvas = New Bitmap(PictureBox1.Width, PictureBox1.Height)
        graphics = Graphics.FromImage(canvas)
        PictureBox1.Image = canvas

        SetDefaultPoints()
        SetDefaultEdges()
        SetDefaultMeshes()

        SetColMat(view, 0, 1, 0, 0, 0)
        SetColMat(view, 1, 0, 1, 0, 0)
        SetColMat(view, 2, 0, 0, 1, 0)
        SetColMat(view, 3, 0, 0, -1 / 4, 1)

        SetColMat(screen, 0, 50, 0, 0, (PictureBox1.Width / 2))
        SetColMat(screen, 1, 0, -50, 0, (PictureBox1.Height / 2))
        SetColMat(screen, 2, 0, 0, 0, 0)
        SetColMat(screen, 3, 0, 0, 0, 1)

        DrawPyramids()
    End Sub

    'reset values for translating and rotating and redraw both pyramids
    Private Sub Reset2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        tetaz = 0
        tetay = 0
        tetax = 0
        dx = -2.5
        dy = 0
        dz = 0
        dx2 = 2
        dy2 = 0
        dz2 = 0
        DrawPyramids()
    End Sub

    'handles keypresses and increases/decreases dx/dy/dz/tetax/tetay/tetaz based on keypress
    Private Sub Form1_KeyDown(ByVal sender As System.Object, ByVal e As KeyEventArgs) Handles MyBase.KeyDown
        ' Sets Handled to true to prevent other controls from 
        ' receiving the key if an arrow key was pressed
        Dim bHandled As Boolean = False

        Select Case e.KeyCode
            Case Keys.D
                If MoveRadio.Checked Then
                    If Pyramid1Radio.Checked Then
                        dx = dx + 0.1
                        DrawPyramids()
                    ElseIf Pyramid2Radio.Checked Then
                        dx2 = dx2 + 0.1
                        DrawPyramids()
                    Else
                        MessageBox.Show("No pyramid is selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    End If
                ElseIf RotateRadio.Checked Then
                    If Pyramid1Radio.Checked Then
                        tetax = tetax + 1
                        DrawPyramids()
                    ElseIf Pyramid2Radio.Checked Then
                        tetax2 = tetax2 + 1
                        DrawPyramids()
                    Else
                        MessageBox.Show("No pyramid is selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    End If
                Else
                    MessageBox.Show("No command is selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                End If
                e.Handled = True
            Case Keys.A
                If MoveRadio.Checked Then
                    If Pyramid1Radio.Checked Then
                        dx = dx - 0.1
                        DrawPyramids()
                    ElseIf Pyramid2Radio.Checked Then
                        dx2 = dx2 - 0.1
                        DrawPyramids()
                    Else
                        MessageBox.Show("No pyramid is selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    End If
                ElseIf RotateRadio.Checked Then
                    If Pyramid1Radio.Checked Then
                        tetax = tetax - 1
                        DrawPyramids()
                    ElseIf Pyramid2Radio.Checked Then
                        tetax2 = tetax2 - 1
                        DrawPyramids()
                    Else
                        MessageBox.Show("No pyramid is selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    End If
                Else
                    MessageBox.Show("No command is selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                End If
                e.Handled = True
            Case Keys.W
                If MoveRadio.Checked Then
                    If Pyramid1Radio.Checked Then
                        dy = dy + 0.1
                        DrawPyramids()
                    ElseIf Pyramid2Radio.Checked Then
                        dy2 = dy2 + 0.1
                        DrawPyramids()
                    Else
                        MessageBox.Show("No pyramid is selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    End If
                ElseIf RotateRadio.Checked Then
                    If Pyramid1Radio.Checked Then
                        tetay = tetay + 1
                        DrawPyramids()
                    ElseIf Pyramid2Radio.Checked Then
                        tetay2 = tetay2 + 1
                        DrawPyramids()
                    Else
                        MessageBox.Show("No pyramid is selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    End If
                Else
                    MessageBox.Show("No command is selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                End If
                e.Handled = True
            Case Keys.S
                If MoveRadio.Checked Then
                    If Pyramid1Radio.Checked Then
                        dy = dy - 0.1
                        DrawPyramids()
                    ElseIf Pyramid2Radio.Checked Then
                        dy2 = dy2 - 0.1
                        DrawPyramids()
                    Else
                        MessageBox.Show("No pyramid is selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    End If
                ElseIf RotateRadio.Checked Then
                    If Pyramid1Radio.Checked Then
                        tetay = tetay - 1
                        DrawPyramids()
                    ElseIf Pyramid2Radio.Checked Then
                        tetay2 = tetay2 - 1
                        DrawPyramids()
                    Else
                        MessageBox.Show("No pyramid is selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    End If
                Else
                    MessageBox.Show("No command is selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                End If
                e.Handled = True
            Case Keys.E
                If MoveRadio.Checked Then
                    If Pyramid1Radio.Checked Then
                        dz = dz - 0.125
                        DrawPyramids()
                    ElseIf Pyramid2Radio.Checked Then
                        dz2 = dz2 - 0.125
                        DrawPyramids()
                    Else
                        MessageBox.Show("No pyramid is selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    End If
                ElseIf RotateRadio.Checked Then
                    If Pyramid1Radio.Checked Then
                        tetax = tetax - 1
                        DrawPyramids()
                    ElseIf Pyramid2Radio.Checked Then
                        tetax2 = tetax2 - 1
                        DrawPyramids()
                    Else
                        MessageBox.Show("No pyramid is selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    End If
                Else
                    MessageBox.Show("No command is selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                End If
                e.Handled = True
            Case Keys.Z
                If MoveRadio.Checked Then
                    If Pyramid1Radio.Checked Then
                        dz = dz + 0.125
                        DrawPyramids()
                    ElseIf Pyramid2Radio.Checked Then
                        dz2 = dz2 + 0.125
                        DrawPyramids()
                    Else
                        MessageBox.Show("No pyramid is selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    End If
                ElseIf RotateRadio.Checked Then
                    If Pyramid1Radio.Checked Then
                        tetaz = tetaz + 1
                        DrawPyramids()
                    ElseIf Pyramid2Radio.Checked Then
                        tetaz2 = tetaz2 + 1
                        DrawPyramids()
                    Else
                        MessageBox.Show("No pyramid is selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    End If
                Else
                    MessageBox.Show("No command is selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                End If
                e.Handled = True
        End Select
    End Sub
    Private Function ClipIntersect(eW As EdgeWindow, N As Point, sV As Vertex, eV As Vertex) As Vertex
        Dim t As Single = ((sV.x - eW.x1) * N.X + (sV.y - eW.y1) * N.Y) / ((sV.x - eV.x) * N.X + (sV.y - eV.y) * N.Y)
        Return New Vertex((sV.x + t * (eV.x - sV.x)), (sV.y + t * (eV.y - sV.y)), (sV.z + t * (eV.z - sV.z)))
    End Function
    Private Function SutherlandHodgmanClip(window As List(Of EdgeWindow), v1 As Vertex, v2 As Vertex, v3 As Vertex) As List(Of Vertex)
        Dim polygonOutput As New List(Of Vertex) From {v1, v2, v3}
        For Each e In window
            Dim polygonInput As New List(Of Vertex)
            polygonInput.AddRange(polygonOutput)
            polygonOutput.Clear()

            Dim startV As Vertex = polygonInput.Last
            For Each endV In polygonInput
                Dim startSide, endSide As Single
                Dim m As Double = Math.Sqrt((e.x2 - e.x1) ^ 2 + (e.y2 - e.y1) ^ 2)
                Dim N As Point

                If m = 0 Then
                    N = New Point(0, 0)
                Else
                    N = New Point((e.x2 - e.x1) / m, (e.y2 - e.y1) / m)
                End If

                startSide = ((startV.x - e.x1) * N.X + (startV.y - e.y1) * N.Y)
                endSide = ((endV.x - e.x1) * N.X + (endV.y - e.y1) * N.Y)

                '>0 Inside | <=0 Outside
                If startSide > 0 And endSide > 0 Then
                    polygonOutput.Add(endV.ToShort)
                ElseIf startSide > 0 And endSide <= 0 Then
                    polygonOutput.Add(ClipIntersect(e, N, startV, endV).ToShort)
                ElseIf startSide <= 0 And endSide > 0 Then
                    polygonOutput.Add(ClipIntersect(e, N, startV, endV).ToShort)
                    polygonOutput.Add(endV.ToShort)
                ElseIf startSide <= 0 And endSide <= 0 Then
                    'Do Nothing
                End If
                startV = endV
            Next

            If polygonOutput.Count = 0 Then
                Return polygonOutput
            End If
        Next
        Return polygonOutput
    End Function
    Private Function Warnock_IsEqual(window As List(Of EdgeWindow), polygon As List(Of Vertex)) As Boolean
        If polygon.Count = 0 Or polygon.Count > 4 Then
            Return False
        End If

        Dim i As Byte = 1
        Dim x1, y1, x2, y2 As Integer
        For Each e In window
            x1 = polygon.Item(i).x
            y1 = polygon.Item(i).y

            If (polygon.Count = 4 And i = 3) Or (polygon.Count = 3 And i = 2) Then
                i = 0
            Else
                i = i + 1
            End If

            x2 = polygon.Item(i).x
            y2 = polygon.Item(i).y

            If e.x1 <> x1 OrElse e.y1 <> y1 OrElse e.x2 <> x2 OrElse e.y2 <> y2 Then
                Return False
            End If
        Next
        Return True
    End Function
    Private Sub Warnock_Fill(window As List(Of EdgeWindow), color As Brush)
        Dim points(3) As Point
        For i = 0 To 3
            points(i) = New Point(window.Item(i).x1, window.Item(i).y1)
        Next
        graphics.FillPolygon(color, points)
    End Sub
    Private Sub Warnock_Fill(polygon As List(Of Vertex), color As Brush)
        Dim i As Byte = polygon.Count
        Dim points(i - 1) As Point
        For i = 0 To i - 1
            points(i) = New Point(polygon.Item(i).x, polygon.Item(i).y)
        Next
        graphics.FillPolygon(color, points)
    End Sub
    Public Sub Warnock(windowBLx As Short, windowBLy As Short, windowTRx As Short, windowTRy As Short, lvl As Byte)
        'Initialize
        Dim window As New List(Of EdgeWindow) From {
                New EdgeWindow(New Point(windowBLx, windowBLy), New Point(windowBLx, windowTRy)),
                New EdgeWindow(New Point(windowBLx, windowTRy), New Point(windowTRx, windowTRy)),
                New EdgeWindow(New Point(windowTRx, windowTRy), New Point(windowTRx, windowBLy)),
                New EdgeWindow(New Point(windowTRx, windowBLy), New Point(windowBLx, windowBLy))
            }
        Dim polygonClip, polygonWarnock As New List(Of PolygonVertex)
        Dim nSur, nIntr As Byte
        Dim Easy As Boolean
        Easy = False
        nSur = 0
        nIntr = 0

        For Each m In mesh1
            Dim v1 As New Vertex(VS1(m.vertex1).x, VS1(m.vertex1).y, VS1(m.vertex1).z)
            Dim v2 As New Vertex(VS1(m.vertex2).x, VS1(m.vertex2).y, VS1(m.vertex2).z)
            Dim v3 As New Vertex(VS1(m.vertex3).x, VS1(m.vertex3).y, VS1(m.vertex3).z)
            polygonClip.Add(New PolygonVertex(SutherlandHodgmanClip(window, v1.ToShort, v2.ToShort, v3.ToShort), Brushes.Yellow))
        Next

        For Each m In mesh2
            Dim v1 As New Vertex(VS2(m.vertex1).x, VS2(m.vertex1).y, VS2(m.vertex1).z)
            Dim v2 As New Vertex(VS2(m.vertex2).x, VS2(m.vertex2).y, VS1(m.vertex2).z)
            Dim v3 As New Vertex(VS2(m.vertex3).x, VS2(m.vertex3).y, VS2(m.vertex3).z)
            polygonClip.Add(New PolygonVertex(SutherlandHodgmanClip(window, v1.ToShort, v2.ToShort, v3.ToShort), Brushes.Azure))
        Next

        'Identify the polygon is surround or intersect the window
        For Each c In polygonClip
            If Warnock_IsEqual(window, c.vertex) Then
                nSur = nSur + 1
                polygonWarnock.Add(c)
            ElseIf c.vertex.Count <> 0 Then
                nIntr = nIntr + 1
                polygonWarnock.Add(c)
            End If
        Next

        'Identify the condition whether 'easy' or 'hard', fill the 'easy' polygon/window
        If nSur = 0 And nIntr = 0 Then
            Warnock_Fill(window, Brushes.White)
            Easy = True
        ElseIf nSur = 1 And nIntr = 0 Then
            Warnock_Fill(window, polygonWarnock.Last.color)
            Easy = True
        ElseIf nSur = 0 And nIntr = 1 Then
            Warnock_Fill(window, Brushes.White)
            Warnock_Fill(polygonWarnock.Last.vertex, polygonWarnock.Last.color)
            Easy = True
        ElseIf nSur >= 1 Then
            Dim polygonFront As PolygonVertex = polygonWarnock.Last
            Dim zMin As Short = polygonFront.Find_Minimum
            Dim zMax As Short = polygonFront.Find_Maximum
            For Each p In polygonWarnock
                Dim _zMin As Short = p.Find_Minimum
                Dim _zMax As Short = p.Find_Maximum
                'Prevent cross edge
                If zMin >= _zMax Or _zMin >= zMax Then
                    If _zMin >= zMax Then
                        polygonFront = p
                        zMin = _zMin
                        zMax = _zMax
                    End If
                    Easy = True
                Else
                    Easy = False
                    Exit For
                End If
            Next

            If Easy = True AndAlso Warnock_IsEqual(window, polygonFront.vertex) Then
                Warnock_Fill(window, polygonFront.color)
            Else
                Easy = False
            End If
        End If

        'Condition if the warnock is too deep
        If Easy = False AndAlso lvl = 0 AndAlso polygonWarnock.Count <> 0 Then
            Dim polygonFront As PolygonVertex = polygonWarnock.Last
            Dim zMin As Short = polygonFront.Find_Minimum
            Dim zMax As Short = polygonFront.Find_Maximum
            For Each cp In polygonWarnock
                Dim _zMin As Short = cp.Find_Minimum
                Dim _zMax As Short = cp.Find_Maximum
                If zMin >= _zMax Or _zMin >= zMax Then
                    If _zMin >= zMax Then
                        polygonFront = cp
                        zMin = _zMin
                        zMax = _zMax
                    End If
                    Easy = True
                Else
                    Easy = False
                End If
            Next
            If Easy = True Then
                Warnock_Fill(window, polygonFront.color)
            Else
                Warnock_Fill(window, polygonWarnock.Last.color)
            End If
            Easy = True
        End If

        'Conditon is hard, split into 4 more deep windows
        If Easy = False Then
            Warnock(windowBLx, windowBLy, windowBLx + (2 ^ (lvl - 1)), windowBLy + (2 ^ (lvl - 1)), lvl - 1)
            Warnock(windowBLx + (2 ^ (lvl - 1)), windowBLy, windowTRx, windowBLy + (2 ^ (lvl - 1)), lvl - 1)
            Warnock(windowBLx, windowBLy + (2 ^ (lvl - 1)), windowBLx + (2 ^ (lvl - 1)), windowTRy, lvl - 1)
            Warnock(windowBLx + (2 ^ (lvl - 1)), windowBLy + (2 ^ (lvl - 1)), windowTRx, windowTRy, lvl - 1)
        End If


    End Sub
End Class