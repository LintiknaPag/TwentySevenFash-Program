﻿Imports System.Data
Imports System.Data.SqlClient
Imports System.Transactions

Public Class Login
    Dim con As New SqlConnection("Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\TwentySevenFash-Program\TwentySevenFash.mdf;Integrated Security=True")
    Dim cmd As SqlCommand

    Private Sub btnLogin_Click(sender As Object, e As EventArgs)


    End Sub

    Private Sub lblForgotPwd_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs)

    End Sub

    Private Sub btnLogin_Click_1(sender As Object, e As EventArgs) Handles btnLogin.Click

        If txtUsername.Text = "" Then
            MsgBox("Enter Username")
            txtUsername.Focus()

        End If
        If txtPassword.Text = "" Then
            MsgBox("Enter Password")
            txtPassword.Focus()
        End If

        Try
            con.Open()
            cmd = New SqlCommand("select * from users where username= @username and password= @password", con)

            cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = txtUsername.Text
            cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = txtPassword.Text

            Dim sda As New SqlDataAdapter(cmd)

            Dim table As New DataTable()

            sda.Fill(table)
            If table.Rows.Count() <= 0 Then
                MsgBox("Enter Valid username and password", MsgBoxStyle.OkOnly, "Error")
            End If
            If table.Rows(0)("USERTYPE") = "admin" Then
                Dim dashboardmain As New Dashboard
                Dashboard.dashboardmain = txtUsername.Text
                dashboardPOS.dashboardmain = txtUsername.Text
                Dashboard.Show()
                Me.Hide()

                Dim objj As New Dashboard
                objj.wlcm = txtUsername.Text.ToUpper
                objj.Show()

            End If
            txtPassword.Text = ""
            txtUsername.Text = ""
            con.Close()
        Catch ex As Exception
            con.Close()
            MsgBox("Error " + ex.Message, MsgBoxStyle.Critical, "Error")
        End Try


    End Sub

End Class
