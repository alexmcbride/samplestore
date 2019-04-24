<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SampleStore.WebForm.Default" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>Sample Store</title>
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css" integrity="sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T" crossorigin="anonymous">
</head>
<body>
    <div class="container">
        <h1>Sample Store</h1>

        <!-- File Upload -->
        <p>Upload a file to the sample store. Make sure you add a sample through the API first.</p>
        <form id="uploadForm" runat="server">
            <div class="form-group">
                <label for="FileUpload">Select a file to upload:</label><br />
                <div class="input-group">
                    <div class="input-group-prepend">
                        <span class="input-group-text" id="inputGroupFileAddon01">Upload</span>
                    </div>
                    <div class="custom-file">
                        <asp:FileUpload CssClass="custom-file-input" ID="FileUpload" runat="server" AllowMultiple="false" />
                        <label class="custom-file-label" for="inputGroupFile01">Choose file</label>
                    </div>
                </div>
            </div>
            <div class="form-group">
                <label for="IdDropDown">Select sample to upload file for:</label>
                <br />
                <asp:DropDownList CssClass="form-control" ID="IdDropDown" SelectMethod="GetSamplesWithoutUploads" runat="server" DataTextField="Text" DataValueField="Value" AppendDataBoundItems="true">
                    <asp:ListItem Text="---- Select Sample ----" Value="" disabled="disabled" Selected="true"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div>
                <asp:Button CssClass="btn btn-primary" ID="UploadButton" runat="server" Text="Upload" OnClick="UploadButton_Click" />
                <asp:Label ID="MessageLabel" runat="server" Text=""></asp:Label>
            </div>

            <hr />

            <!-- Sample List -->
            <h2>Samples</h2>
            <p>A list up previously uploaded samples.</p>
            <asp:ListView ID="SampleList" runat="server" SelectMethod="GetSamplesWithUploads" DataKeyNames="SampleID" ItemType="SampleStore.Models.Sample" ItemPlaceholderID="SoundsPlaceholder">
                <LayoutTemplate>
                    <div class="list-group">
                        <asp:PlaceHolder runat="server" ID="SoundsPlaceholder"></asp:PlaceHolder>
                    </div>
                </LayoutTemplate>
                <ItemTemplate>
                    <div class="list-group-item">
                        <p>
                            <strong>
                                <asp:Literal Text='<%# Eval("Title") %>' runat="server" /> - 
                                <asp:Literal Text='<%# Eval("Artist") %>' runat="server" />
                            </strong>
                        </p>
                        <audio src='<%# Eval("SampleMp3Url") %>' controls="" preload="none"></audio>
                    </div>
                </ItemTemplate>
            </asp:ListView>
            <hr />
            <asp:Button CssClass="btn btn-secondary" ID="RefreshButton" runat="server" Text="Refresh" OnClick="RefreshButton_Click" />
        </form>
    </div>
</body>
</html>
