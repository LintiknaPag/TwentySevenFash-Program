﻿Imports System.Data.SqlClient
Imports System.Security.Policy
Imports System.Windows.Documents
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar
Imports FontAwesome.Sharp


Public Class dashboardPOS
    Dim con As New SqlConnection("Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\TwentySevenFash-Program\TwentySevenFash.mdf;Integrated Security=True")
    Dim cmd As SqlCommand
    Dim dr As SqlDataReader
    Dim newButton As Button
    Dim total As Double
    Public Shared dashboardmain
    Public adminName As String = dashboardmain.ToString
    Dim lastRecord As Double


    Private Sub IconButton1_Click(sender As Object, e As EventArgs) Handles IconButton1.Click

        con.Open()

        cmd = New SqlCommand("select * from Items where ID= @ID", con)
        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = TextBox1.Text
        dr = cmd.ExecuteReader

        While dr.Read
            total += CDbl(dr.Item("price").ToString)
            DataGridView1.Rows.Add(dr.Item("Id").ToString, dr.Item("ItemName").ToString, Format(CDbl(dr.Item("Price").ToString), "#,##0.00"))
        End While
        dr.Close()
        con.Close()
        lblTotal.Text = Format(total, "#,##.00")



    End Sub

    Private Sub dashboardPOS_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        LoadItems()

    End Sub

    Private Sub LoadItems()
        con.Open()
        cmd = New SqlCommand("select * from items", con)
        dr = cmd.ExecuteReader
        Dim count As Integer
        Dim myFont As System.Drawing.Font
        myFont = New System.Drawing.Font("Impact", 12)
        FlowLayoutPanel1.Controls.Clear()
        While dr.Read
            newButton = New Button()

            With newButton
                .BackColor = Color.FromArgb(46, 51, 73)
                .ForeColor = Color.White
                .Size = New Size(200, 123)
                .Name = "btnItem" + count.ToString()
                .Tag = dr.Item("ID").ToString()
                .Text = dr.Item("ItemName").ToString()
                .Font = myFont
                .FlatStyle = FlatStyle.Flat
                .FlatAppearance.BorderSize = 3
                .FlatAppearance.BorderColor = Color.White
            End With

            FlowLayoutPanel1.Controls.Add(newButton)

            count += 1
            AddHandler newButton.Click, AddressOf DynamicButton_click
        End While




        con.Close()
    End Sub

    Public Sub DynamicButton_click(ByVal sender As Object, ByVal e As EventArgs)
        Try
            con.Open()
            cmd = New SqlCommand("select * from Items where ID= @ID", con)
            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = sender.tag.ToString
            dr = cmd.ExecuteReader
            While dr.Read
                lastRecord = dr.Item("price")
                total += CDbl(dr.Item("price").ToString)
                DataGridView1.Rows.Add(dr.Item("Id").ToString, dr.Item("ItemName").ToString, Format(CDbl(dr.Item("Price").ToString), "#,##0.00"))
            End While
            dr.Close()
            con.Close()
            lblTotal.Text = Format(total, "#,##.00")
        Catch ex As Exception
            MsgBox("Database Error", MsgBoxStyle.Critical)
        End Try




    End Sub

    Private Sub IconButton2_Click(sender As Object, e As EventArgs) Handles IconButton2.Click
        DataGridView1.Rows.Clear()
        lblTotal.Text = "0.00"
        total = 0
    End Sub

    Private Sub IconButton4_Click(sender As Object, e As EventArgs) Handles IconButton4.Click
        con.Open()
        Dim county As New SqlCommand("select count(*) from Sales", con)
        Dim count1 = Convert.ToInt16(county.ExecuteScalar)

        con.Close()
        Dim rand As New Random
        Dim transcID As Integer = count1
        Dim confirmation As String = MsgBox("Checkout?", MsgBoxStyle.YesNo, "Confirmation")
        Dim change As Double
        If confirmation = vbYes Then
            Dim cash As Double = InputBox("Cash: ", "Cash")
            If cash < total Then
                MsgBox("Unsufficient Balance!")
            Else
                change = cash - total
            End If

            Try
                cmd = New SqlCommand("insert into Sales values(@DateofTransaction,@ID,@Revenue,@Expenses,@Profit,@SoldItems)", con)
                cmd.Parameters.AddWithValue("@DateofTransaction", DateString)
                cmd.Parameters.AddWithValue("@ID", transcID)
                cmd.Parameters.AddWithValue("@Revenue", total)
                cmd.Parameters.AddWithValue("@Expenses", total.ToString)
                cmd.Parameters.AddWithValue("@Profit", total)
                cmd.Parameters.AddWithValue("@SoldItems", DataGridView1.Rows.Count.ToString)


                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()

                MsgBox("Transaction Complete" + Environment.NewLine() + "Change: " + Format(change, "#,##.00"))
                lblTotal.Text = "0.00"
                total = 0
                DataGridView1.Rows.Clear()
            Catch ex As Exception
                con.Close()
                MsgBox(ex.Message, MsgBoxStyle.Critical, "error")
            End Try
        End If


    End Sub

    Private Sub IconButton3_Click(sender As Object, e As EventArgs) Handles IconButton3.Click

        If MsgBox("Void last Item?", vbYesNo + vbQuestion, "Confirmation") = vbYes Then
            Try
                Dim password As String = InputBox("Password:", "Verification")
                con.Open()
                cmd = New SqlCommand("select * from users where username= @username and password= @password", con)
                cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = adminName.ToString
                cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = password.ToString
                Dim sda As New SqlDataAdapter(cmd)
                Dim table As New DataTable
                sda.Fill(table)

                If table.Rows.Count() <= 0 Then
                    MsgBox("Password incorrect", MsgBoxStyle.OkOnly, "Wrong Password")
                Else
                    total -= lastRecord
                    lblTotal.Text = Format(total, "#,##.00")
                    DataGridView1.Rows.RemoveAt(DataGridView1.Rows.Count - 1)
                End If
                con.Close()
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.Critical, "Error")
            End Try

        End If


    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        LoadItems()
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged

        If TextBox1.Text <> String.Empty Then
            con.Open()
            cmd = New SqlCommand("select * from items where id= @ID", con)
            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = TextBox1.Text
            dr = cmd.ExecuteReader
            Dim count As Integer
            Dim myFont As System.Drawing.Font
            myFont = New System.Drawing.Font("Impact", 12)
            FlowLayoutPanel1.Controls.Clear()
            Dim myFont As System.Drawing.Font
            myFont = New System.Drawing.Font("Impact", 12)
            While dr.Read
                newButton = New Button()

                With newButton
                    .BackColor = Color.FromArgb(46, 51, 73)
                    .ForeColor = Color.White
                    .Size = New Size(200, 123)
                    .Name = "btnItem" + count.ToString()
                    .Tag = dr.Item("ID").ToString()
                    .Text = dr.Item("ItemName").ToString()
                    .Font = myFont
                    .FlatStyle = FlatStyle.Flat
                    .FlatAppearance.BorderSize = 3
                    .FlatAppearance.BorderColor = Color.White
                End With

                FlowLayoutPanel1.Controls.Add(newButton)

                count += 1
                AddHandler newButton.Click, AddressOf DynamicButton_click


            End While
            con.Close()
        ElseIf TextBox1.Text = String.Empty Then
            LoadItems()
        End If



    End Sub

    Private Sub FlowLayoutPanel1_Paint(sender As Object, e As PaintEventArgs) Handles FlowLayoutPanel1.Paint

    End Sub
End Class