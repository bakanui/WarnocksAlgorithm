Public Class Form1
    Dim graphics As Graphics
    Dim canvas As Bitmap
    Dim vertex1(3), vertex2(4) As Vertex
    Dim edge1(5), edge2(8) As Edge
    Dim mesh1(3), mesh2(5) As Mesh
    Dim pr(3, 3), pr2(3, 3) As Single
    Dim Translate(3, 3), Rotatex(3, 3), Rotatey(3, 3), Rotatez(3, 3) As Single
    Dim Translate2(3, 3), Rotatex2(3, 3), Rotatey2(3, 3), Rotatez2(3, 3) As Single
    Dim view(3, 3), screen(3, 3) As Single
    Dim dx, dy, dz, tetax, tetay, tetaz As Single
    Dim dx2, dy2, dz2, tetax2, tetay2, tetaz2 As Single

    Dim VR(7), VS(7) As Point
    Dim deg As Single = 0

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.KeyPreview = True
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

        DrawPyramids(True, True)
    End Sub
    Structure Vector
        Public i, j, k As Single
    End Structure

    Sub SetVector(ByRef vector As Vector, x As Single, y As Single, z As Single)
        vector.i = x
        vector.j = y
        vector.k = z
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        tetaz = 0
        tetay = 0
        tetax = 0
        dx = -2.5
        dy = 0
        dz = 0
        dx2 = 2
        dy2 = 0
        dz2 = 0
        DrawPyramids(True, True)
    End Sub

    Structure Edge
        Dim point1, point2 As Integer
    End Structure
    Structure Vertex
        Dim x, y, z, w As Single
    End Structure
    Structure Mesh
        Dim vertex1, vertex2, vertex3 As Integer
        Dim isVisible As Boolean
    End Structure

    Sub SetMesh(ByRef mesh As Mesh, ByVal sf1 As Integer, ByVal sf2 As Integer, ByVal sf3 As Integer)
        mesh.vertex1 = sf1
        mesh.vertex2 = sf2
        mesh.vertex3 = sf3
    End Sub
    Sub SetColMat(ByRef Matrix(,) As Single, col As Integer, a As Double, b As Double, c As Double, d As Double)
        Matrix(0, col) = a
        Matrix(1, col) = b
        Matrix(2, col) = c
        Matrix(3, col) = d
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
    Function DegreeToRadian(ByRef degree As Integer)
        Return degree * Math.PI / 180
    End Function

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
    Function cos(x As Single) As Single
        Return Math.Cos(x * (Math.PI / 180))
    End Function

    Function sin(x As Single) As Single
        Return Math.Sin(x * (Math.PI / 180))
    End Function

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
    Function DotProductof(V1 As Vector, V2 As Vector) As Single
        Return (V1.i * V2.i) + (V1.j * V2.j) + (V1.k * V2.k)
    End Function
    Sub BackFaceCulling(ByRef surface1() As Mesh, ByRef surface2() As Mesh, coor1() As Vertex, coor2() As Vertex, Upd1 As Boolean, Upd2 As Boolean)
        Dim N, V As New Vector

        SetVector(V, 0, 0, -4)

        If Upd1 Then
            For i = 0 To 3
                N = Normalof(surface1(i), coor1)
                If DotProductof(V, N) >= 0 Then
                    surface1(i).isVisible = False
                Else
                    surface1(i).isVisible = True
                End If
            Next
        End If

        If Upd2 Then
            For i = 0 To 4
                N = Normalof(surface2(i), coor2)
                If DotProductof(V, N) >= 0 Then
                    surface2(i).isVisible = False
                Else
                    surface2(i).isVisible = True
                End If
            Next
        End If
    End Sub
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
    Function CrossProductof(V1 As Vector, V2 As Vector) As Vector
        Dim result As New Vector
        result.i = (V1.j * V2.k) - (V1.k * V2.j)
        result.j = (V1.k * V2.i) - (V1.i * V2.k)
        result.k = (V1.i * V2.j) - (V1.j * V2.i)
        Return result
    End Function
    Sub DrawPyramids(Update1 As Boolean, Update2 As Boolean)
        Dim VR1(3), VS1(3), VR2(4), VS2(4) As Vertex
        Dim a, b, c, d, e, f, g, h, j, k, l, m As Integer
        graphics.Clear(PictureBox1.BackColor)

        If Update1 Then
            SetColMat(Translate, 0, 1, 0, 0, dx)
            SetColMat(Translate, 1, 0, 1, 0, dy)
            SetColMat(Translate, 2, 0, 0, 1, dz)
            SetColMat(Translate, 3, 0, 0, 0, 1)
            'rotate

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
            For i = 0 To 3
                pr = MultiplyMat2(Rotatez, Rotatey)
                pr = MultiplyMat2(pr, Rotatex)
                pr = MultiplyMat2(pr, Translate)
                VR1(i) = New Vertex
                VR1(i) = MultiplyMat1(vertex1(i), pr)
                VS1(i) = MultiplyMat1(VR1(i), view)
                VS1(i) = MultiplyMat1(VS1(i), screen)
            Next

            For i = 0 To 3
                a = VS1(mesh1(i).vertex1).x
                b = VS1(mesh1(i).vertex1).y
                c = VS1(mesh1(i).vertex2).x
                d = VS1(mesh1(i).vertex2).y
                graphics.DrawLine(Pens.Black, a, b, c, d)
                e = VS1(mesh1(i).vertex2).x
                f = VS1(mesh1(i).vertex2).y
                g = VS1(mesh1(i).vertex3).x
                h = VS1(mesh1(i).vertex3).y
                graphics.DrawLine(Pens.Black, e, f, g, h)
                j = VS1(mesh1(i).vertex3).x
                k = VS1(mesh1(i).vertex3).y
                l = VS1(mesh1(i).vertex1).x
                m = VS1(mesh1(i).vertex1).y
                graphics.DrawLine(Pens.Black, j, k, l, m)
            Next
        End If

        If Update2 Then
            'rotation matrix for pyramid 1

            SetColMat(Rotatez2, 0, cos(tetaz2), -sin(tetaz2), 0, 0)
            SetColMat(Rotatez2, 1, sin(tetaz2), cos(tetaz2), 0, 0)
            SetColMat(Rotatez2, 2, 0, 0, 1, 0)
            SetColMat(Rotatez2, 3, 0, 0, 0, 1)


            SetColMat(Rotatey2, 0, cos(tetay2), 0, sin(tetay2), 0)
            SetColMat(Rotatey2, 1, 0, 1, 0, 0)
            SetColMat(Rotatey2, 2, -sin(tetay2), 0, cos(tetay2), 0)
            SetColMat(Rotatey2, 3, 0, 0, 0, 1)


            SetColMat(Rotatex2, 0, 1, 0, 0, 0)
            SetColMat(Rotatex2, 1, 0, cos(tetax2), -sin(tetax2), 0)
            SetColMat(Rotatex2, 2, 0, sin(tetax2), cos(tetax2), 0)
            SetColMat(Rotatex2, 3, 0, 0, 0, 1)

            SetColMat(Translate2, 0, 1, 0, 0, dx2)
            SetColMat(Translate2, 1, 0, 1, 0, dy2)
            SetColMat(Translate2, 2, 0, 0, 1, dz2)
            SetColMat(Translate2, 3, 0, 0, 0, 1)
            For i = 0 To 4
                pr2 = MultiplyMat2(Rotatez2, Rotatey2)
                pr2 = MultiplyMat2(pr2, Rotatex2)
                pr2 = MultiplyMat2(pr2, Translate2)
                VR2(i) = New Vertex
                VR2(i) = MultiplyMat1(vertex2(i), pr2)
                VS2(i) = MultiplyMat1(VR2(i), view)
                VS2(i) = MultiplyMat1(VS2(i), screen)
            Next

            For i = 0 To 4
                a = VS2(mesh2(i).vertex1).x
                b = VS2(mesh2(i).vertex1).y
                c = VS2(mesh2(i).vertex2).x
                d = VS2(mesh2(i).vertex2).y
                graphics.DrawLine(Pens.Blue, a, b, c, d)
                e = VS2(mesh2(i).vertex2).x
                f = VS2(mesh2(i).vertex2).y
                g = VS2(mesh2(i).vertex3).x
                h = VS2(mesh2(i).vertex3).y
                graphics.DrawLine(Pens.Blue, e, f, g, h)
                j = VS2(mesh2(i).vertex3).x
                k = VS2(mesh2(i).vertex3).y
                l = VS2(mesh2(i).vertex1).x
                m = VS2(mesh2(i).vertex1).y
                graphics.DrawLine(Pens.Blue, j, k, l, m)
            Next
        End If

        BackFaceCulling(mesh1, mesh2, VS1, VS2, Update1, Update2)
        'graph.Clear(Canvas.BackColor)
        PictureBox1.Refresh()
    End Sub

    Private Sub Form1_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        ' Sets Handled to true to prevent other controls from 
        ' receiving the key if an arrow key was pressed
        Dim bHandled As Boolean = False

        Select Case e.KeyCode
            Case Keys.D
                If MoveRadio.Checked Then
                    If Pyramid1Radio.Checked Then
                        dx = dx + 0.1
                        SetColMat(Translate, 0, 1, 0, 0, dx)
                    ElseIf Pyramid2Radio.Checked Then
                        dx2 = dx2 + 0.1
                        SetColMat(Translate2, 0, 1, 0, 0, dx2)
                    Else
                        MessageBox.Show("No pyramid is selected.",
                                        "Warning",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Exclamation,
                                        MessageBoxDefaultButton.Button1)
                    End If
                ElseIf RotateRadio.Checked Then
                    If Pyramid1Radio.Checked Then
                        tetax = tetax + 1
                        SetColMat(Rotatex, 1, 0, cos(tetax), -sin(tetax), 0)
                        SetColMat(Rotatex, 2, 0, sin(tetax), cos(tetax), 0)
                    ElseIf Pyramid2Radio.Checked Then
                        tetax2 = tetax2 + 1
                        SetColMat(Rotatex2, 1, 0, cos(tetax2), -sin(tetax2), 0)
                        SetColMat(Rotatex2, 2, 0, sin(tetax2), cos(tetax2), 0)
                    Else
                        MessageBox.Show("No pyramid is selected.",
                                        "Warning",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Exclamation,
                                        MessageBoxDefaultButton.Button1)
                    End If
                Else
                    MessageBox.Show("No command is selected.",
                                    "Warning",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Exclamation,
                                    MessageBoxDefaultButton.Button1)
                End If
                DrawPyramids(True, True)
                e.Handled = True
            Case Keys.A
                If MoveRadio.Checked Then
                    If Pyramid1Radio.Checked Then
                        dx = dx - 1
                        SetColMat(Translate, 0, 1, 0, 0, dx)
                    ElseIf Pyramid2Radio.Checked Then
                        dx2 = dx2 - 1
                        SetColMat(Translate, 0, 1, 0, 0, dx2)
                    Else
                        MessageBox.Show("No pyramid is selected.",
                                        "Warning",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Exclamation,
                                        MessageBoxDefaultButton.Button1)
                    End If
                ElseIf RotateRadio.Checked Then
                    If Pyramid1Radio.Checked Then
                        tetax = tetax - 1
                        SetColMat(Rotatex, 1, 0, cos(tetax), -sin(tetax), 0)
                        SetColMat(Rotatex, 2, 0, sin(tetax), cos(tetax), 0)
                    ElseIf Pyramid2Radio.Checked Then
                        tetax2 = tetax2 - 1
                        SetColMat(Rotatex2, 1, 0, cos(tetax2), -sin(tetax2), 0)
                        SetColMat(Rotatex2, 2, 0, sin(tetax2), cos(tetax2), 0)
                    Else
                        MessageBox.Show("No pyramid is selected.",
                                        "Warning",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Exclamation,
                                        MessageBoxDefaultButton.Button1)
                    End If
                Else
                    MessageBox.Show("No command is selected.",
                                    "Warning",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Exclamation,
                                    MessageBoxDefaultButton.Button1)
                End If
                DrawPyramids(True, True)
                e.Handled = True
            Case Keys.W
                If MoveRadio.Checked Then
                    If Pyramid1Radio.Checked Then
                        dy = dy + 0.1
                        SetColMat(Translate, 1, 0, 1, 0, dy)
                    ElseIf Pyramid2Radio.Checked Then
                        dy2 = dy2 + 0.1
                        SetColMat(Translate2, 1, 0, 1, 0, dy2)
                    Else
                        MessageBox.Show("No pyramid is selected.",
                                        "Warning",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Exclamation,
                                        MessageBoxDefaultButton.Button1)
                    End If
                ElseIf RotateRadio.Checked Then
                    If Pyramid1Radio.Checked Then
                        tetay = tetay + 1
                        SetColMat(Rotatey, 0, cos(tetay), 0, sin(tetay), 0)
                        SetColMat(Rotatey, 2, -sin(tetay), 0, cos(tetay), 0)
                    ElseIf Pyramid2Radio.Checked Then
                        tetay2 = tetay2 + 1
                        SetColMat(Rotatey2, 0, cos(tetay2), 0, sin(tetay2), 0)
                        SetColMat(Rotatey2, 2, -sin(tetay2), 0, cos(tetay2), 0)
                    Else
                        MessageBox.Show("No pyramid is selected.",
                                        "Warning",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Exclamation,
                                        MessageBoxDefaultButton.Button1)
                    End If
                Else
                    MessageBox.Show("No command is selected.",
                                    "Warning",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Exclamation,
                                    MessageBoxDefaultButton.Button1)
                End If
                DrawPyramids(True, True)
                e.Handled = True
            Case Keys.S
                If MoveRadio.Checked Then
                    If Pyramid1Radio.Checked Then
                        dy = dy - 0.1
                        SetColMat(Translate, 1, 0, 1, 0, dy)
                    ElseIf Pyramid2Radio.Checked Then
                        dy2 = dy2 - 0.1
                        SetColMat(Translate2, 1, 0, 1, 0, dy2)
                    Else
                        MessageBox.Show("No pyramid is selected.",
                                        "Warning",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Exclamation,
                                        MessageBoxDefaultButton.Button1)
                    End If
                ElseIf RotateRadio.Checked Then
                    If Pyramid1Radio.Checked Then
                        tetay = tetay - 1
                        SetColMat(Rotatey, 0, cos(tetay), 0, sin(tetay), 0)
                        SetColMat(Rotatey, 2, -sin(tetay), 0, cos(tetay), 0)
                    ElseIf Pyramid2Radio.Checked Then
                        tetay2 = tetay2 - 1
                        SetColMat(Rotatey2, 0, cos(tetay2), 0, sin(tetay2), 0)
                        SetColMat(Rotatey2, 2, -sin(tetay2), 0, cos(tetay2), 0)
                    Else
                        MessageBox.Show("No pyramid is selected.",
                                        "Warning",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Exclamation,
                                        MessageBoxDefaultButton.Button1)
                    End If
                Else
                    MessageBox.Show("No command is selected.",
                                    "Warning",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Exclamation,
                                    MessageBoxDefaultButton.Button1)
                End If
                DrawPyramids(True, True)
                e.Handled = True
            Case Keys.E
                If MoveRadio.Checked Then
                    If Pyramid1Radio.Checked Then
                        dz = dz - 0.125
                        SetColMat(Translate, 2, 0, 0, 1, dz)
                    ElseIf Pyramid2Radio.Checked Then
                        dz2 = dz2 - 0.125
                        SetColMat(Translate2, 2, 0, 0, 1, dz2)
                    Else
                        MessageBox.Show("No pyramid is selected.",
                                        "Warning",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Exclamation,
                                        MessageBoxDefaultButton.Button1)
                    End If
                ElseIf RotateRadio.Checked Then
                    If Pyramid1Radio.Checked Then
                        tetax = tetax - 1
                        SetColMat(Rotatez, 0, cos(tetaz), -sin(tetaz), 0, 0)
                        SetColMat(Rotatez, 1, sin(tetaz), cos(tetaz), 0, 0)
                    ElseIf Pyramid2Radio.Checked Then
                        tetax2 = tetax2 - 1
                        SetColMat(Rotatez, 0, cos(tetaz), -sin(tetaz), 0, 0)
                        SetColMat(Rotatez, 1, sin(tetaz), cos(tetaz), 0, 0)
                    Else
                        MessageBox.Show("No pyramid is selected.",
                                        "Warning",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Exclamation,
                                        MessageBoxDefaultButton.Button1)
                    End If
                Else
                    MessageBox.Show("No command is selected.",
                                    "Warning",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Exclamation,
                                    MessageBoxDefaultButton.Button1)
                End If
                DrawPyramids(True, True)
                e.Handled = True
            Case Keys.Z
                If MoveRadio.Checked Then
                    If Pyramid1Radio.Checked Then
                        dz = dz + 0.125
                        SetColMat(Translate, 2, 0, 0, 1, dz)
                    ElseIf Pyramid2Radio.Checked Then
                        dz2 = dz2 + 0.125
                        SetColMat(Translate2, 2, 0, 0, 1, dz2)
                    Else
                        MessageBox.Show("No pyramid is selected.",
                                        "Warning",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Exclamation,
                                        MessageBoxDefaultButton.Button1)
                    End If
                ElseIf RotateRadio.Checked Then
                    If Pyramid1Radio.Checked Then
                        tetaz = tetaz + 1
                        SetColMat(Rotatez, 0, cos(tetaz), -sin(tetaz), 0, 0)
                        SetColMat(Rotatez, 1, sin(tetaz), cos(tetaz), 0, 0)
                    ElseIf Pyramid2Radio.Checked Then
                        tetaz2 = tetaz2 + 1
                        SetColMat(Rotatez, 0, cos(tetaz), -sin(tetaz), 0, 0)
                        SetColMat(Rotatez, 1, sin(tetaz), cos(tetaz), 0, 0)
                    Else
                        MessageBox.Show("No pyramid is selected.",
                                        "Warning",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Exclamation,
                                        MessageBoxDefaultButton.Button1)
                    End If
                Else
                    MessageBox.Show("No command is selected.",
                                    "Warning",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Exclamation,
                                    MessageBoxDefaultButton.Button1)
                End If
                DrawPyramids(True, True)
                e.Handled = True
        End Select
    End Sub
End Class