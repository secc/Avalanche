<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AvalancheConfiguration.ascx.cs" Inherits="RockWeb.Plugins.Avalanche.AvalancheConfiguration" %>

<asp:UpdatePanel ID="pnlConfiguration" runat="server" Class="panel panel-block">
    <ContentTemplate>
        <Rock:ModalDialog runat="server" ID="mdPerson" SaveButtonText="Save" Title="Person Attributes to Send To App" OnSaveClick="mdPerson_SaveClick">
            <Content>
                <Rock:RockCheckBoxList runat="server" ID="cblPerson" Label="Attributes" DataValueField="Id" DataTextField="Name"></Rock:RockCheckBoxList>
            </Content>
        </Rock:ModalDialog>

        <Rock:ModalDialog runat="server" ID="mdGroupTypes" SaveButtonText="Save" Title="Group Types to Pass Membership With" OnSaveClick="mdGroupTypes_SaveClick">
            <Content>
                If the app user is in a group of these group types it will be passed on to the app.
                <Rock:GroupTypesPicker runat="server" ID="gtpGroupTypes" Label="Group Types" />
            </Content>
        </Rock:ModalDialog>

        <Rock:ModalDialog runat="server" ID="mdGroups" SaveButtonText="Save" OnSaveClick="mdGroups_SaveClick">
            <Content>
                If the app user is in one of the following selected groups it will be passed on to the app.
                <Rock:GroupPicker runat="server" ID="gpGroups" Label="Groups" AllowMultiSelect="true" />
                <Rock:RockCheckBoxList runat="server" ID="gtpSecurity" Label="Security Groups" DataTextField="Name" DataValueField="Id" />
            </Content>
        </Rock:ModalDialog>

        <div class="panel-heading">
            <h1 class="panel-title">
                <i class="fa fa-mobile"></i>
                Avalanche Configuration</h1>
        </div>
        <div class="panel-body">
            <asp:ValidationSummary ID="vsDetails" runat="server" HeaderText="Please Correct the Following" CssClass="alert alert-danger" />
            <div class="row">
                <div class="col-md-6">
                    <Rock:PagePicker runat="server" ID="ppHome" Required="true" Label="Home Page" />
                </div>
                <div class="col-md-6">
                    <Rock:PagePicker ID="ppFooter" runat="server" Label="Footer Page" />
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <Rock:KeyValueList runat="server" Label="App Attributes" ID="kvAttributes" />
                </div>
            </div>
            <hr />
            <div class="row">
                <div class="col-md-12">
                    <Rock:RockLiteral runat="server" ID="lPerson" Label="Person Attributes"></Rock:RockLiteral>
                    <Rock:BootstrapButton runat="server" ID="btnPerson" Text="Edit" CssClass="btn btn-default btn-sm"
                        OnClick="btnPerson_Click" />
                </div>
            </div>
            <hr />
            <div class="row">
                <div class="col-md-12">
                    <Rock:RockLiteral runat="server" ID="lGroupTypes" Label="Group Types"></Rock:RockLiteral>
                    <Rock:BootstrapButton runat="server" ID="btnGroupTypes" Text="Edit" CssClass="btn btn-default btn-sm"
                        OnClick="btnGroupTypes_Click" />
                </div>
            </div>
            <hr />
            <div class="row">
                <div class="col-md-12">
                    <Rock:RockLiteral runat="server" ID="lGroups" Label="Groups"></Rock:RockLiteral>
                    <Rock:BootstrapButton runat="server" ID="btnGroups" Text="Edit" CssClass="btn btn-default btn-sm"
                        OnClick="btnGroups_Click" />
                </div>
            </div>
            <br />
            <br />
            <Rock:BootstrapButton runat="server" ID="btnSave" CssClass="btn btn-primary" Text="Save" OnClick="btnSave_Click" />
            <Rock:BootstrapButton runat="server" ID="btnBack" CssClass="btn btn-link" Text="Cancel" OnClick="btnBack_Click" />
        </div>
    </ContentTemplate>
</asp:UpdatePanel>

