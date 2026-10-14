

    var searchTimeout;
    var detailsSearchPageNo = 1;
    var detailsSearchPageSize = 10;
    var detailsIsLoading = false;
    var detailsHasMore = true;
    var detailsCurrentSearch = "";

    $(document).ready(function () {
        GetComplaintBankList();
        //GetComplaintCurrentStatus();
        //GetDefendantList();
        //GetComplaintReplyToList();
        //GetLogList();
        //GetReplyList(0);
        //$("#txtOrderSummary").val('');
        var now = new Date();
        var day = ("0" + now.getDate()).slice(-2);
        var month = ("0" + (now.getMonth() + 1)).slice(-2);
        var today = now.getFullYear() + "-" + (month) + "-" + (day);
        $('#txtTransactionDate').val(today);
        $('#txtReceiptDate').val(today);

        var currentYear = new Date().getFullYear();
        for (var i = 0; i <= 10; i++) {
            $('#drpSearchComplaintYear').append('<option value="' + (currentYear - i) + '">' + (currentYear - i) + '</option>');
        }
        $('#drpSearchComplaintYear').val(new Date().getFullYear());

        //if (!$(e.target).closest('.position-relative').length) {
        //    $('#advocateSearchResults').hide();
        //}
    });

    $('#btnSearchComplaint').on('click', function () {
        var numberPart = $('#txtSearchComplaintNumber').val().trim();
        var yearPart = $('#drpSearchComplaintYear').val().trim();

        if (numberPart === '' || yearPart === '') {
            swal('Please enter Complaint Number and select Year.', '', 'error');
            return;
        }

        var fullComplaintNumber = 'BCG/CMP/' + numberPart + '/' + yearPart;

        $.ajax({
            type: 'POST',
            url: '/Complaint/SearchComplaintByNumber',
            data: { ComplaintNumber: fullComplaintNumber },
            success: function (response) {
                if (response.IsSuccess && response.Data != null) {
                    var data = response.Data;
                    console.log(data);
                    // Set hidden fields
                    $('#hdfComplaintId').val(data.Id);
                    $('#hdfStatus').val(data.Status || '');

                    $('#divPaymentDetailList').empty();


                    var DueAmount = data.DueAmount == null ? '0' : data.DueAmount == '' ? '0' : data.DueAmount;

                    // Fill Complainant Details
                    $('#lblComplainantFullName').text(data.FullName || '');
                    $('#lblComplainantEmail').text(data.EmailAddress || '');
                    $('#lblComplainantContact').text(data.ContactNumber || '');
                    $('#lblComplainantAddress').text(data.Address || '');
                    $('#lblComplaintDateTime').text(data.SCreateDate || '');
                    $('#txtComplaintStatus').text(data.Status || '');
                    updateActionButtonsUI(data.Status || '');
                    $('#lblTotalAmount').text(data.TotalAmount || '');
                    $('#lblPaidAmount').text(data.PaidAmount || '');
                    $('#lblDueAmount').text(DueAmount);

                    if (parseFloat(DueAmount) > 0) {
                        $('#divPayPayment').removeClass("d-none");
                    }
                    else {
                        $('#divPayPayment').addClass("d-none");
                    }

                    // Fill Complaint Against List
                    $('#selectDetails').empty();
                    if (data.ComplaintAgainstList != null && data.ComplaintAgainstList !== '') {
                        try {
                            var againstList = JSON.parse(data.ComplaintAgainstList);
                            $.each(againstList, function (i, item) {
                                var srNo = i + 1;

                                var EnrollmentNumberTextId = 'txtEnrollmentNumber' + srNo;
                                var EnrollmentIdHiddenId = 'hdfEnrollmentId' + srNo;

                                var VSelectDetails = '';
                                var VIsKnownNo = '';
                                if (item.IsKnown === 'Y' && item.EnrollmentNumber != null && item.EnrollmentNumber !== '') {
                                    VIsKnownNo = 'nonumber';
                                    VSelectDetails = '<div class="complaint_number"><div class="title">Enrollment Number</div><a class="description">' + item.EnrollmentNumber + '</a></div>';
                                }
                                else {
                                    VIsKnownNo = 'number';
                                    VSelectDetails = '<input id=' + EnrollmentNumberTextId + ' type="text" class="form-control txtenrollmentnumberwith" placeholder="Enrollment Number"  onchange=EnrollmentNumberOnChange(' + srNo + ') value=""><input id=' + EnrollmentIdHiddenId + ' type="hidden" value=' + item.Id + '>';
                                }

                                var extraDetails = '';
                                if (item.IsKnown === 'N') {
                                    extraDetails = '</div><div class="row">'
                                                 + '<div class="col-md-6 mt-2 d-flex flex-column"><div class="title">City</div><div class="description descriptionborderbottom mt-auto">' + (item.City || '&nbsp;') + '</div></div>'
                                                 + '<div class="col-md-6 mt-2 d-flex flex-column"><div class="title">Mobile No.</div><div class="description descriptionborderbottom mt-auto">' + (item.MobileNo || '&nbsp;') + '</div></div>';
                                }

                                var againstHtml = '<div class="col-md-4 mb-3"><div class="main_card_list main_card_list_border" ><div class="main_header_card">'
                                    + '<div class="_left_side">'
                                        + '<div class="' + VIsKnownNo + '">' + srNo + '</div>'
                                        + VSelectDetails
                                    + '</div>'
                                    + '<div class="rigth_side"></div>'
                                    + '</div>'
                                    + '<div class="main_card_list_body">'
                                        + '<div class="row">'
                                            + '<div class="col-md-12 mt-2">'
                                                + '<div class="title">Full Name</div>'
                                                + '<div class="description descriptionborderbottom">' + (item.AdvocateName || '') + '</div>'
                                            + '</div>'
                                            + extraDetails
                                        + '</div>'
                                    + '</div>'
                                    + '<div class="main_card_list_body">'
                                        + '<div class="row">'
                                            + '<div class="col-md-12 mt-2 text-end">'
                                                + '<div class="title">Amount: ' + (item.Amount || 0) + '</div>'
                                            + '</div>'
                                        + '</div>'
                                    + '</div>'
                                + '</div></div>';

                                $('#selectDetails').append(againstHtml);
                            });
                        } catch (e) { }
                    }

                    // Fill Affidavit Copy
                    $('#dvAffidavitCopy').empty();
                    if (data.ComplaintAffidavitCopyList != null && data.ComplaintAffidavitCopyList !== '') {
                        try {
                            var affidavitList = JSON.parse(data.ComplaintAffidavitCopyList);
                            $.each(affidavitList, function (i, item) {
                                if (item.FilePath != null && item.FilePath !== '') {
                                    var affHtml = '<div class="main_card_list main_card_list_border p-12-custome mt-3">'
                                        + '<div class="main_card_list_body"><div class="row"><div class="col-md-12">'
                                        + '<div class="file_name_btn" onclick="getFile(\'' + item.FilePath + '\')"><div class="name">' + (item.FileName || '') + '</div><div class="view_file"><i class="las la-eye"></i></div></div>'
                                        + '</div></div></div></div>';
                                    $('#dvAffidavitCopy').append(affHtml);
                                }
                            });
                        } catch (e) { }
                    }

                    // Fill Additional Attachments
                    $('#dvOrderPassed').empty();
                    if (data.ComplaintAdditionalAttachmentsList != null && data.ComplaintAdditionalAttachmentsList !== '') {
                        try {
                            var additionalList = JSON.parse(data.ComplaintAdditionalAttachmentsList);
                            $('#dvOrderPassed').empty();
                            $.each(additionalList, function (i, item) {
                                var fileBlock = '';
                                if (item.FilePath != null && item.FilePath !== '') {
                                    fileBlock = '<div class="file_name_btn" onclick="getFile(\'' + item.FilePath + '\')"><div class="name">' + (item.FileName || '') + '</div><div class="view_file"><i class="las la-eye"></i></div></div>';
                                }
                                var noteBlock = '';
                                if (item.Description != null && item.Description.trim() !== '') {
                                    noteBlock = '<div class="col-md-12 mt-3"><div class="title">Note (નોંધ)</div><div class="description">' + item.Description + '</div></div>';
                                }
                                var addHtml = '<div class="main_card_list main_card_list_border p-12-custome mt-3">'
                                    + '<div class="main_card_list_body"><div class="row">'
                                    + '<div class="col-md-12"><div class="title">Upload File (ફાઇલ અપલોડ)</div>' + fileBlock + '</div>'
                                    + noteBlock
                                    + '</div></div></div>';
                                $('#dvOrderPassed').append(addHtml);
                            });
                        } catch (e) { }
                    }

                    // Fill Payment Details
                    if (data.ComplaintPaymentList != null && data.ComplaintPaymentList !== '') {
                        try {
                            var compaymentList = JSON.parse(data.ComplaintPaymentList);
                            if (compaymentList.length > 0) {
                                $.each(compaymentList, function (i, item) {
                                    var paymentitemload = '';
                                    var hideCashDetails = (item.PaymentMode && item.PaymentMode.toUpperCase() === 'CASH') ? 'd-none' : '';
                                    paymentitemload = '<div class="row">'
                                                        + '<div class="col-md-4 mt-1">'
                                                            + '<div class="form-group">'
                                                                + '<label>Receipt Number</label>'
                                                                + '<label class="form-control">' + item.ReceiptNumber + '</label>'
                                                            + '</div>'
                                                        + '</div>'
                                                        + '<div class="col-md-4 mt-1">'
                                                            + '<div class="form-group">'
                                                                + '<label>Receipt Date</label>'
                                                                + '<label class="form-control">' + item.ReceiptDate + '</label>'
                                                            + '</div>'
                                                        + '</div>'
                                                        + '<div class="col-md-4 mt-1">'
                                                            + '<div class="form-group">'
                                                                + '<label>Paid Amount</label>'
                                                                + '<label class="form-control">' + item.PaidAmount + '</label>'
                                                            + '</div>'
                                                        + '</div>'
                                                        + '<div class="col-md-4 mt-1">'
                                                            + '<div class="form-group">'
                                                                + '<label>Receipt By</label>'
                                                                + '<label class="form-control">' + item.PaymentMode + '</label>'
                                                            + '</div>'
                                                        + '</div>'
                                                        + '<div class="col-md-4 mt-1 ' + hideCashDetails + '">'
                                                            + '<div class="form-group">'
                                                                + '<label>Transaction No.</label>'
                                                                + '<label class="form-control">' + item.TransactionNumber + '</label>'
                                                            + '</div>'
                                                        + '</div>'
                                                        + '<div class="col-md-4 mt-1">'
                                                            + '<div class="form-group">'
                                                                + '<label>Transaction Date</label>'
                                                                + '<label class="form-control">' + item.TransactionDate + '</label>'
                                                            + '</div>'
                                                        + '</div>'
                                                        + '<div class="col-md-12 mt-1 ' + hideCashDetails + '">'
                                                            + '<div class="form-group descriptionborderbottom mb-2">'
                                                                + '<label>Bank</label>'
                                                                + '<label class="form-control">' + item.BankName + '</label>'
                                                            + '</div>'
                                                        + '</div>'
                                                    + '</div>';

                                    $('#divPaymentDetailList').append(paymentitemload);
                                });

                                $('#divPaymentDetail').removeClass('d-none');
                            }
                            else {
                                $('#divPaymentDetailList').empty();
                                $('#divPaymentDetail').addClass('d-none');
                            }
                        }
                        catch (e) {
                            $('#divPaymentDetailList').empty();
                            $('#divPaymentDetail').addClass('d-none');
                        }
                    }
                    else {
                        $('#divPaymentDetailList').empty();
                        $('#divPaymentDetail').addClass('d-none');
                    }

                    if (data.ComplainedAgainstReplyToList != null && data.ComplainedAgainstReplyToList !== '') {
                        try {
                            var ComplainedAgainstReplyToList = JSON.parse(data.ComplainedAgainstReplyToList);
                            if (ComplainedAgainstReplyToList.length > 0) {
                                $.each(ComplainedAgainstReplyToList, function (data, row) {
                                    var FilePath = "'" + row.FilePath + "'";
                                    var deleteBtnHtml = '';
                                    if (row.IsShowDeleteButton === 'Y') {
                                        deleteBtnHtml = '<button type="button" class="btn btn-outline-danger btn-sm ms-2 py-0" style="font-size: 11px; line-height: 1.2;" onclick="deleteReply(\'' + row.Id + '\')"><i class="fa-solid fa-trash me-1"></i>Delete</button>';
                                    }
                                    var RowItem = '<div class="replaydetailsmain">'
                                                    + '<div class="Sub_titile d-flex justify-content-between sendernamestyle">'
                                                        + '<div class="fw-bold font-size-17 text-dark"><i class="fa-solid fa-user text-muted me-2"></i>' + row.SenderName + '</div>'
                                                        + '<div class="d-flex align-items-center">'
                                                            + '<span class="fw-bold font-size-17 text-dark"><i class="fa-regular fa-clock text-muted me-2"></i>' + row.SCreateDate + ' ' + row.SCreateTime + '</span>'
                                                            + deleteBtnHtml
                                                        + '</div>'
                                                    + '</div>';

                                    if (row.Summery != null && row.Summery.trim() !== "") {
                                        RowItem += '<div class="Sub_titile mt-2">'
                                                         + 'Summery | Short Information'
                                                    + '</div>'
                                                    + '<div class="main_contentupdate">'
                                                         + '<p>' + row.Summery + '</p>'
                                                    + '</div>';
                                    }

                                    if (row.Comprehensive != null && row.Comprehensive.trim() !== "") {
                                        RowItem += '<div class="Sub_titile mt-3">'
                                                         + 'Comprehensive Complaint'
                                                    + '</div>'
                                                    + '<div class="main_contentupdate">'
                                                         + '<p>' + row.Comprehensive + '</p>'
                                                    + '</div>';
                                    }

                                    if (row.FilePath != null && row.FilePath.trim() !== "") {
                                        RowItem += '<div class="Sub_titile mt-3">Attachment</div>'
                                                        + '<div class="main_contentupdate">'
                                                            + '<div class="fileupload_details">'
                                                                + '<div class="d-flex align-items-center" onclick="getFile(\'' + row.FilePath + '\')" style="cursor:pointer;" title="Click to view file">'
                                                                    + '<div class="pdf-btn btn-sm">'
                                                                        + '<img src="/assets/img/pdficon.svg">'
                                                                    + '</div>'
                                                                    + '<div class="fileupload_name text-primary text-decoration-underline">' + row.FileName + '</div>'
                                                                + '</div>'
                                                            + '</div>'
                                                        + '</div>';
                                    }

                                    RowItem += '</div>';
                                    $('#dvreplyData').append(RowItem);
                                });
                            }
                            else {
                                $('#dvreplyData').empty();
                                $('#dvreplyData').append('No data available');
                            }
                        }
                        catch (e) {
                            $('#dvreplyData').empty();
                            $('#dvreplyData').append('No data available');
                        }
                    }
                    else {
                        $('#dvreplyData').empty();
                        $('#dvreplyData').append('No data available');
                    }

                    // Show details area
                    $('#dvComplaintDetailsArea').removeClass('hide-element');
                    getGBMeetingLogs(data.Id);
                } else {
                    $('#dvComplaintDetailsArea').addClass('hide-element');
                    swal(response.Message || 'Complaint not found.', '', 'error');
                }
            },
            error: function () {
                swal('Error searching complaint.', '', 'error');
            }
        });
    });

    $('#btnClearSearch').on('click', function () {
        $('#txtSearchComplaintNumber').val('');
        $('#drpSearchComplaintYear').val(new Date().getFullYear());
        $('#dvComplaintDetailsArea').addClass('hide-element');
        $('#dvGBMeetingList').empty();
    });

    function ComplaintComplainantsDetailsByComplaintNumberRefresh() {
        var numberPart = $('#txtSearchComplaintNumber').val().trim();
        var yearPart = $('#drpSearchComplaintYear').val().trim();

        if (numberPart === '' || yearPart === '') {
            swal('Please enter Complaint Number and select Year.', '', 'error');
            return;
        }

        var fullComplaintNumber = 'BCG/CMP/' + numberPart + '/' + yearPart;

        $.ajax({
            type: 'POST',
            url: '/Complaint/SearchComplaintComplainantsDetailsByComplaintNumber',
            data: { ComplaintNumber: fullComplaintNumber },
            success: function (response) {
                if (response.IsSuccess && response.Data != null) {
                    var data = response.Data;
                    $('#hdfComplaintId').val(data.Id);
                    $('#hdfStatus').val(data.Status || '');
                    $('#lblComplainantFullName').text(data.FullName || '');
                    $('#lblComplainantEmail').text(data.EmailAddress || '');
                    $('#lblComplainantContact').text(data.ContactNumber || '');
                    $('#lblComplainantAddress').text(data.Address || '');
                    $('#txtComplaintStatus').text(data.Status || '');
                    updateActionButtonsUI(data.Status || '');
                } else {
                    $('#hdfComplaintId').val('0');
                    $('#hdfStatus').val('');
                    $('#lblComplainantFullName').text('');
                    $('#lblComplainantEmail').text('');
                    $('#lblComplainantContact').text('');
                    $('#lblComplainantAddress').text('');
                    $('#txtComplaintStatus').text('');
                    updateActionButtonsUI('');
                }
            },
            error: function () {
                $('#hdfComplaintId').val('0');
                $('#hdfStatus').val('');
                $('#lblComplainantFullName').text('');
                $('#lblComplainantEmail').text('');
                $('#lblComplainantContact').text('');
                $('#lblComplainantAddress').text('');
                $('#txtComplaintStatus').text('');
                    updateActionButtonsUI('');
            }
        });
    }

    function isNumberKey(evt, element) {
        var charCode = (evt.which) ? evt.which : event.keyCode
        if (charCode >= 48 && charCode <= 57)
            return true;
        else {
            swal("Please enter valid number.", "", "error");
            return false;
        }
        return false;
    }

    function openFiledialog() {
        $("#fileLoader").click();
    }

    function GetComplaintBankList() {
        $.ajax({
            type: "POST",
            url: "/Complaint/GetComplaintBankList",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                $("#ddlBank").empty();
                $("#ddlBank").append('<option value="0">Select</option>');
                if (response.IsSuccess) {
                    if (response.Data != null) {
                        if (response.Data != '') {

                            $.each(response.Data, function (i, item) {
                                $("#ddlBank").append('<option value="' + item.Id + '">' + item.BankName + '</option>');
                            });
                        }
                        else {
                            $("#ddlBank").empty();
                            $("#ddlBank").append('<option value="0">Select</option>');
                        }
                    }
                    else {
                        $("#ddlBank").empty();
                        $("#ddlBank").append('<option value="0">Select</option>');
                    }
                }
                else {
                    $("#ddlBank").empty();
                    $("#ddlBank").append('<option value="0">Select</option>');
                }
            }, failure: function (result) {
                $("#ddlBank").empty();
                $("#ddlBank").append('<option value="0">Select</option>');
            },
            error: function (result) {
                $("#ddlBank").empty();
                $("#ddlBank").append('<option value="0">Select</option>');
            }
        });
    }

    function GetComplaintPaymentReceivedLog() {
        $('#divPaymentDetailList').empty();
        $.ajax({
            type: "POST",
            url: "/Complaint/GetComplaintPaymentReceivedLog",
            data: JSON.stringify({ ComplaintId: $("#hdfComplaintId").val() }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.IsSuccess) {
                    if (response.Data != null) {
                        if (response.Data != '') {
                            $.each(response.Data, function (i, item) {
                                var paymentitemnew = '';
                                var hideCashDetailsNew = (item.PaymentMode && item.PaymentMode.toUpperCase() === 'CASH') ? 'd-none' : '';
                                paymentitemnew = '<div class="row">'
                                                    + '<div class="col-md-4 mt-1">'
                                                        + '<div class="form-group">'
                                                            + '<label>Receipt Number</label>'
                                                            + '<label class="form-control">' + item.ReceiptNumber + '</label>'
                                                        + '</div>'
                                                    + '</div>'
                                                    + '<div class="col-md-4 mt-1">'
                                                        + '<div class="form-group">'
                                                            + '<label>Receipt Date</label>'
                                                            + '<label class="form-control">' + item.ReceiptDate + '</label>'
                                                        + '</div>'
                                                    + '</div>'
                                                    + '<div class="col-md-4 mt-1">'
                                                            + '<div class="form-group">'
                                                                + '<label>Paid Amount</label>'
                                                                + '<label class="form-control">' + item.PaidAmount + '</label>'
                                                            + '</div>'
                                                        + '</div>'
                                                    + '<div class="col-md-4 mt-1">'
                                                        + '<div class="form-group">'
                                                            + '<label>Receipt By</label>'
                                                            + '<label class="form-control">' + item.PaymentMode + '</label>'
                                                        + '</div>'
                                                    + '</div>'
                                                    + '<div class="col-md-4 mt-1 ' + hideCashDetailsNew + '">'
                                                        + '<div class="form-group">'
                                                            + '<label>Transaction No.</label>'
                                                            + '<label class="form-control">' + item.TransactionNumber + '</label>'
                                                        + '</div>'
                                                    + '</div>'
                                                    + '<div class="col-md-4 mt-1">'
                                                        + '<div class="form-group">'
                                                            + '<label>Transaction Date</label>'
                                                            + '<label class="form-control">' + item.TransactionDate + '</label>'
                                                        + '</div>'
                                                    + '</div>'
                                                    + '<div class="col-md-12 mt-1 ' + hideCashDetailsNew + '">'
                                                        + '<div class="form-group descriptionborderbottom mb-2">'
                                                            + '<label>Bank</label>'
                                                            + '<label class="form-control">' + item.BankName + '</label>'
                                                        + '</div>'
                                                    + '</div>'
                                                + '</div>';

                                $('#divPaymentDetailList').append(paymentitemnew);
                                $('#divPaymentDetail').removeClass('d-none');
                            });
                        }
                        else {
                            $('#divPaymentDetailList').empty();
                            $('#divPaymentDetail').addClass('d-none');
                        }
                    }
                    else {
                        $('#divPaymentDetailList').empty();
                        $('#divPaymentDetail').addClass('d-none');
                    }
                }
                else {
                    $('#divPaymentDetailList').empty();
                    $('#divPaymentDetail').addClass('d-none');
                }
            }, failure: function (result) {
                $('#divPaymentDetailList').empty();
                $('#divPaymentDetail').addClass('d-none');
            },
            error: function (result) {
                $('#divPaymentDetailList').empty();
                $('#divPaymentDetail').addClass('d-none');
            }
        });
    }

    function GetComplaintCurrentStatus() {
        $.ajax({
            type: "POST",
            url: "/Complaint/GetComplaintCurrentStatus",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ ComplaintId: $("#hdfComplaintId").val() }),
            dataType: "json",
            success: function (response) {
                if (response.IsSuccess) {
                    if (response.Data != null) {
                        if (response.Data != '') {
                            var VComplaintCurrentStatus = response.Data[0].Status;
                            $('#txtComplaintStatus').text(VComplaintCurrentStatus);
                    updateActionButtonsUI(VComplaintCurrentStatus);
                        }
                        else {
                            var VComplaintCurrentStatus = '';
                            $('#txtComplaintStatus').text(VComplaintCurrentStatus);
                    updateActionButtonsUI(VComplaintCurrentStatus);
                        }
                    }
                    else {
                        var VComplaintCurrentStatus = '';
                        $('#txtComplaintStatus').text(VComplaintCurrentStatus);
                    updateActionButtonsUI(VComplaintCurrentStatus);
                    }
                }
                else {
                    var VComplaintCurrentStatus = '';
                    $('#txtComplaintStatus').text(VComplaintCurrentStatus);
                    updateActionButtonsUI(VComplaintCurrentStatus);
                }
            }, failure: function (result) {
                swal('Something went wrong!', '', 'error');
            },
            error: function (result) {
                swal('Something went wrong!', '', 'error');
            }
        });
    }

    function GetComplaintPaymentDetail() {
        $.ajax({
            type: "POST",
            url: "/Complaint/GetComplaintPaymentDetail",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ ComplaintId: $("#hdfComplaintId").val() }),
            dataType: "json",
            success: function (response) {
                if (response.IsSuccess) {
                    if (response.Data != null) {
                        if (response.Data != '') {
                            var VTotalAmount = response.Data[0].TotalAmount;
                            var VPaidAmount = response.Data[0].PaidAmount;
                            var VDueAmount = response.Data[0].DueAmount == null ? '0' : response.Data[0].DueAmount == '' ? '0' : response.Data[0].DueAmount;
                            var VPaymentStatus = response.Data[0].PaymentStatus;
                            var VStatus = response.Data[0].Status;

                            $('#lblTotalAmount').html(VTotalAmount);
                            $('#lblPaidAmount').html(VPaidAmount);
                            $('#lblDueAmount').html(VDueAmount);

                            if (parseFloat(VDueAmount) > 0) {
                                $('#divPayPayment').removeClass("d-none");
                            }
                            else {
                                $('#divPayPayment').addClass("d-none");
                            }
                        }
                        else {
                            $('#lblTotalAmount').html('0');
                            $('#lblPaidAmount').html('0');
                            $('#lblDueAmount').html('0');
                        }
                    }
                    else {
                        $('#lblTotalAmount').html('0');
                        $('#lblPaidAmount').html('0');
                        $('#lblDueAmount').html('0');
                    }
                }
                else {
                    $('#lblTotalAmount').html('0');
                    $('#lblPaidAmount').html('0');
                    $('#lblDueAmount').html('0');
                }
            }, failure: function (result) {
                $('#lblTotalAmount').html('0');
                $('#lblPaidAmount').html('0');
                $('#lblDueAmount').html('0');
            },
            error: function (result) {
                $('#lblTotalAmount').html('0');
                $('#lblPaidAmount').html('0');
                $('#lblDueAmount').html('0');
            }
        });
    }

    function ddlPaymentModeChange() {
        var val = $('#ddlPaymentMode').val();
        if (val == "D.D." || val == "OUTSTANDING" || val == "RTGS" || val == "JOURNAL") {
            $('#divTransactionNumber').css('display', 'block');
            $('#divBank').css('display', 'block');
        } else {
            $('#divTransactionNumber').css('display', 'none');
            $('#divBank').css('display', 'none');
        }
    }

    $('#lnkSavePayment').on('click', function () {

        var IsSave = 1;
        var VDueAmount = parseFloat($('#lblDueAmount').text() == null ? '0' : $('#lblDueAmount').text() == '' ? '0' : $('#lblDueAmount').text());
        var VReceiptNumber = $('#txtReceiptNumber').val() == null ? '' : $.trim($('#txtReceiptNumber').val());
        var VReceiptDate = $('#txtReceiptDate').val() == null ? '' : $.trim($('#txtReceiptDate').val());
        var VPaymentMode = $('#ddlPaymentMode').val() == null ? '' : $('#ddlPaymentMode').val() == 'Select' ? '' : $('#ddlPaymentMode').val() == '0' ? '' : $('#ddlPaymentMode').val();
        var VTransactionNumber = $('#txtTransactionNumber').val() == null ? '' : $.trim($('#txtTransactionNumber').val());
        var VTransactionDate = $('#txtTransactionDate').val() == null ? '' : $.trim($('#txtTransactionDate').val());
        var VBankId = $('#ddlBank').val() == null ? '0' : $('#ddlBank').val() == 'Select' ? '0' : $('#ddlBank').val() == '' ? '0' : $('#ddlBank').val();

        if (VDueAmount <= 0) {
            swal("The due amount is zero.", "", "error");
            IsSave = 0;
            return;
        }
        if (VReceiptNumber == '') {
            swal("Please input receipt number.", "", "error");
            IsSave = 0;
            return;
        }
        if (VReceiptDate == '') {
            swal("Please input receipt date.", "", "error");
            IsSave = 0;
            return;
        }
        if (VPaymentMode == '') {
            swal("Please select receipt by.", "", "error");
            IsSave = 0;
            return;
        }

        if (VPaymentMode == 'CASH') {
            if (VTransactionDate == '') {
                swal("Please input transaction date.", "", "error");
                IsSave = 0;
                return;
            }
        } else if (VPaymentMode == 'D.D.' || VPaymentMode == 'OUTSTANDING' || VPaymentMode == 'RTGS' || VPaymentMode == 'JOURNAL') {
            if (VTransactionDate == '') {
                swal("Please input transaction date.", "", "error");
                IsSave = 0;
                return;
            }
            if (VTransactionNumber == '') {
                swal("Please input transaction number.", "", "error");
                IsSave = 0;
                return;
            }
            if (parseInt(VBankId) == 0) {
                swal("Please select bank.", "", "error");
                IsSave = 0;
                return;
            }
        }

        if (IsSave == 1) {
            var data = JSON.stringify({
                ComplaintId: $("#hdfComplaintId").val(),
                PaidAmount: VDueAmount,
                PaidOn: VReceiptDate,
                PaymentMode: VPaymentMode,
                TransactionNumber: VTransactionNumber,
                TransactionDate: VTransactionDate,
                BankId: VBankId,
                ReceiptNumber: VReceiptNumber,
                ReceiptDate: VReceiptDate
            });

            $.when(SubmitComplaintPayment(data))
            .then(function (response) {
            }).fail(function (err) {
                console.log(err);
            });
        }
    });

    function SubmitComplaintPayment(data) {
        return $.ajax({
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            type: 'POST',
            url: "/Complaint/GetComplaintPaymentReceived",
            data: data,
            success: function (response) {
                if (response.IsSuccess) {
                    if (response.Data != null) {
                        if (response.Data != '') {
                            swal('Complaint payment received successfully.', '', 'success');
                            $("#txtReceiptNumber").val();
                            $("#txtReceiptDate").val();
                            $("#ddlPaymentMode").val('0');
                            $("#txtTransactionDate").val();
                            $("#txtTransactionNumber").val();
                            $("#ddlBank").val('0');

                            $('#divTransactionNumber').removeClass('hide-element');
                            $('#divBank').removeClass('hide-element');

                            GetComplaintPaymentDetail();
                            GetComplaintPaymentReceivedLog();

                            $("#txtComplaintStatus").text(response.Data[0].ComplaintStatus)
                        }
                        else {
                            swal('Opps! Something Went Wrong.', '', 'error');
                        }
                    }
                    else {
                        swal('Opps! Something Went Wrong.', '', 'error');
                    }
                }
                else {
                    swal('Opps! Something Went Wrong.', '', 'error');
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                swal('Opps! Something Went Wrong.', '', 'error');
            }
        });
    }

    function GetDefendantList() {
        $.ajax({
            type: "POST",
            url: "/Complaint/GetOtherList",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ ComplaintId: $("#hdfComplaintId").val() }),
            dataType: "json",
            success: function (response) {
                if (response.IsSuccess) {
                    if (response.Data != null) {

                        $('#selectDetails').append('');

                        var TotalAmount = 0;
                        $.each(response.Data, function (i, row) {

                            var srNo = i + 1;

                            var EnrollmentNumberTextId = 'txtEnrollmentNumber' + srNo;
                            var EnrollmentIdHiddenId = 'hdfEnrollmentId' + srNo;

                            var EnrollmentNumberClass = '';

                            var VSelectDetails = '';
                            if (row.EnrollmentNo != null) {
                                if (row.EnrollmentNo != '') {
                                    EnrollmentNumberClass = 'nonumber';
                                    VSelectDetails = '<div class="complaint_number"><div class="title">Enrollment Number</div><a class="description">' + row.EnrollmentNo + '</a></div>';
                                }
                                else {
                                    EnrollmentNumberClass = 'number';
                                    VSelectDetails = '<input id=' + EnrollmentNumberTextId + ' type="text" class="form-control" placeholder="Enrollment Number"  onchange=EnrollmentNumberOnChange(' + srNo + ') value=""><input id=' + EnrollmentIdHiddenId + ' type="hidden" value=' + row.Id + '>';
                                }
                            }
                            else {
                                EnrollmentNumberClass = 'number';
                                VSelectDetails = '<input id=' + EnrollmentNumberTextId + ' type="text" class="form-control" placeholder="Enrollment Number"  onchange=EnrollmentNumberOnChange(' + srNo + ') value=""><input id=' + EnrollmentIdHiddenId + ' type="hidden" value=' + row.Id + '>';
                            }

                            var selectDetails = '';
                            selectDetails = '<div class="col-md-4 mb-3"><div class="main_card_list main_card_list_border" ><div class="main_header_card">'
                                             + '<div class="_left_side">'
                                                 + '<div class="' + EnrollmentNumberClass + '">' + srNo + '</div>'
                                                     + VSelectDetails
                                                 + '</div>'
                                                 + '<div class="rigth_side"></div>'
                                             + '</div>'
                                             + '<div class="main_card_list_body">'
                                                 + '<div class="row">'
                                                     + '<div class="col-md-12 mt-2">'
                                                         + '<div class="title">Full Name</div>'
                                                         + '<div class="description">' + row.Name + '</div>'
                                                     + '</div>'
                                                 + '</div>'
                                             + '</div>'
                                             + '<div class="main_card_list_body">'
                                                 + '<div class="row">'
                                                     + '<div class="col-md-12 mt-2 text-end">'
                                                         + '<div class="title">Amount: ' + row.Amount + '</div>'
                                                     + '</div>'
                                                 + '</div>'
                                             + '</div>'
                                         + '</div></div>';
                            $('#selectDetails').append(selectDetails);

                            TotalAmount = TotalAmount + parseFloat(row.Amount == null ? '0' : row.Amount == '' ? '0' : row.Amount);
                        });

                        $("#lblManualFinalAmnt").text(TotalAmount);
                    }
                }
                else {

                }
            }, failure: function (result) {
                swal('Something went wrong!', '', 'error');
            },
            error: function (result) {
                swal('Something went wrong!', '', 'error');
            }
        });
    }

    function EnrollmentNumberOnChange(SrNo) {
        var EnrollmentNumberTextId = '#txtEnrollmentNumber' + SrNo;
        var EnrollmentIdHiddenId = '#hdfEnrollmentId' + SrNo;
        var VId = $(EnrollmentIdHiddenId).val()
        var VEnrollmentNumber = $(EnrollmentNumberTextId).val();

        var data = JSON.stringify({
            Id: VId,
            EnrollmentNumber: VEnrollmentNumber
        });

        $.when(SubmitComplaintAgainstUpdate(data))
         .then(function (response) {
         }).fail(function (err) {
             console.log(err);
         });
    };

    function SubmitComplaintAgainstUpdate(data) {
        return $.ajax({
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            type: 'POST',
            url: "/Complaint/ComplaintAgainstUpdate",
            data: data,
            success: function (response) {
                if (response.IsSuccess) {
                    if (response.Data != null) {
                        if (response.Data != '') {
                            var VReturnStatus = response.Data[0].ReturnStatus == null ? '' : response.Data[0].ReturnStatus;
                            if (VReturnStatus == 'UD') {
                                swal('Complaint against detail update successfully.', '', 'success');
                                GetDefendantList();
                            }
                        }
                        else {
                            swal('Opps! Something Went Wrong.', '', 'error');
                        }
                    }
                    else {
                        swal('Opps! Something Went Wrong.', '', 'error');
                    }
                }
                else {
                    swal('Opps! Something Went Wrong.', '', 'error');
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                swal('Opps! Something Went Wrong.', '', 'error');
            }
        });
    }

    function GetComplaintReplyToList() {
        $.ajax({
            type: "POST",
            url: "/Complaint/GetOtherList",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ ComplaintId: $("#hdfComplaintId").val() }),
            dataType: "json",
            success: function (response) {
                if (response.IsSuccess) {
                    if (response.Data != null) {
                        if (response.Data[0].IsKnown == "Y") {

                            $.each(response.Data, function (i, row) {
                                var srNo = i + 1;
                                if (srNo == 1) {
                                    $('#myTab').append('<li class="nav-item" role="presentation">'
                                        + '<input type="hidden" id="hdfSenderId" value=' + row.AdvocateId + ' />'
                                        + '<a class="nav-link active" id="defendantreplay-tab" data-bs-toggle="tab" data-bs-target="#defendantreplay"  role="tab" aria-controls="defendantreplay" aria-selected="false" onclick="GetReplyList(' + row.AdvocateId + ')">' + row.Name + '</a>'
                                    + '</li>');
                                }
                                else {
                                    $('#myTab').append('<li class="nav-item" role="presentation">'
                                        + '<input type="hidden" id="hdfSenderId" value=' + row.AdvocateId + ' />'
                                        + '<a class="nav-link" id="defendantreplay-tab" data-bs-toggle="tab" data-bs-target="#defendantreplay"  role="tab" aria-controls="defendantreplay" aria-selected="false" onclick="GetReplyList(' + row.AdvocateId + ')">' + row.Name + '</a>'
                                    + '</li>');
                                }

                                if (srNo == 1) {
                                    GetReplyList(row.AdvocateId)
                                    $("#defendantreplay").addClass("active show");
                                }
                            });
                        }
                        else {
                            $('#myTab').append('');
                        }
                    }
                }
                else {

                }
            }, failure: function (result) {
                swal('Something went wrong!', '', 'error');
            },
            error: function (result) {
                swal('Something went wrong!', '', 'error');
            }
        });
    }

    function openDcMemberListView(dcId) {
        if (!dcId) return;
        var hdfComplaintIds = '#hdfComplaintid' + dcId;
        var tblDCMemberDetailsid = '#tblDCMemberDetails' + dcId;
        var iconId = '#icon_dcMember_' + dcId;
        var detailsTableHead = $(tblDCMemberDetailsid + " thead");
        var detailsTableBody = $(tblDCMemberDetailsid + " tbody");

        // Toggle feature: if data is already filled, clicking again will hide it
        if ($(detailsTableBody).find("tr").length > 0) {
            $(detailsTableHead).empty();
            $(detailsTableBody).empty();
            $(iconId).removeClass('la-eye-slash').addClass('la-eye');
            return;
        }

        $(detailsTableHead).empty();
        $(detailsTableBody).empty();

        var VComplaintId = $(hdfComplaintIds).val();
        var Data = JSON.stringify({
            ComplaintId: VComplaintId
        });
        $.ajax({
            type: 'POST',
            url: "/Complaint/GetComplaintMemberLogList",
            data: Data,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response != null && response.Data != null && response.Data.length > 0) {
                    $(iconId).removeClass('la-eye').addClass('la-eye-slash');
                    var RowItem = '<tr><th style="text-align: center;">#</th><th>Enrollment Number</th><th>Name</th><th>Contact Number</th></tr>';
                    detailsTableHead.append(RowItem);

                    $.each(response.Data, function (index, mvalue) {
                        var RId = index + 1;
                        var RowItem1 = '<tr><td style="text-align: center;">' + RId + '</td><td>' + mvalue.EnroNumber + '</td><td>' + mvalue.FullName + '</td><td>' + mvalue.ContactNumber + '</td></tr>';
                        detailsTableBody.append(RowItem1);
                    });
                }
            },
            error: function (result) {
                $(detailsTableHead).empty();
                $(detailsTableBody).empty();
                $(iconId).removeClass('la-eye-slash').addClass('la-eye');
                swal('Something went wrong!', '', 'error');
            }
        });
    }

    function GetLogList() {
        var Data = JSON.stringify({
            ComplaintId: $("#hdfComplaintId").val()
        });
        $.ajax({
            type: 'POST',
            url: "/Complaint/GetLogList",
            data: Data,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {

                if (response.IsSuccess) {
                    $('#logDetails').empty();
                    $.each(response.Data, function (data, value) {
                        var FilePath = "'" + value.OrderFilePath + "'";
                        var oldStatusHtml = '';
                        if (value.OldStatus != null && value.OldStatus != '') {
                            oldStatusHtml = '<div class="complaint_number old_status me-0"><div class="title">Old Status</div><a class="description">' + value.OldStatus + '</a></div>';
                        }

                        if (value.Status == "Adjunt") {
                            $('#logDetails').append(
                                '<div class="status_card_details"><div class="check_element"><i class="las la-check-circle"></i></div><div class="main_card_list main_card_list_border main_card_list_border_log w-100"><div class="main_header_card border-0 pb-0"><div class="rigth_side"><div class="user_name_new">' + value.Name + '</div><div class="date_update">' + value.SNextDate + '</div></div><div class="_left_side"><div class="complaint_number paymentstatuspaid"><div class="title">Current Status</div><a class="description">' + value.Status + '</a></div>' + oldStatusHtml + '</div></div><div class="main_card_list_body"><div class="row"><div class="col-md-3 mt-2"><div class="title">Next Date</div><div class="file_name_btn next_date_btn"><div class="name">' + value.SNextDate + '</div></div></div><div class="col-md-12 mt-3"><div class="title">Summary</div><div class="description">' + value.Summary + '</div></div></div></div></div></div>'
                            );
                        }
                        else if (value.Status == "Order Passed") {
                            $('#logDetails').append(
                                '<div class="status_card_details"><div class="check_element"><i class="las la-check-circle"></i></div><div class="main_card_list main_card_list_border main_card_list_border_log w-100"><div class="main_header_card border-0 pb-0"><div class="rigth_side"><div class="user_name_new">' + value.Name + '</div><div class="date_update">' + value.SNextDate + '</div></div><div class="_left_side"><div class="complaint_number paymentstatuspaid"><div class="title">Current Status</div><a class="description">' + value.Status + '</a></div>' + oldStatusHtml + '</div></div><div class="main_card_list_body"><div class="row"><div class="col-md-12 mt-2"><div class="title">File Name</div><div class="file_name_btn" onclick="getFile(' + FilePath + ')"><div class="name">' + value.OrderFileName + '</div><div class="view_file"><i class="las la-eye"></i></div></div></div><div class="col-md-12 mt-3"><div class="title">Summary</div><div class="description">' + value.Summary + '</div></div></div></div></div></div>'
                            );
                        }
                        else if (value.Status == "Board Has Approved") {
                            var tblDCMemberDetailsid = 'tblDCMemberDetails' + value.Id;
                            var hdfCommitteeIds = 'hdfCommitteeId' + value.Id;
                            var hdfComplaintIds = 'hdfComplaintid' + value.Id;
                            var logDetailsitem = '<div class="status_card_details">'
                                                    + '<div class="check_element">'
                                                        + '<i class="las la-check-circle"></i>'
                                                    + '</div>'
                                                    + '<div class="main_card_list main_card_list_border main_card_list_border_log w-100">'
                                                        + '<div class="main_header_card border-0 pb-0">'
                                                            + '<div class="rigth_side">'
                                                                + '<div class="user_name_new">' + value.Name + '</div>'
                                                                + '<div class="date_update">' + value.SNextDate + '</div>'
                                                            + '</div>'
                                                            + '<div class="_left_side">'
                                                                + '<div class="complaint_number paymentstatuspaid">'
                                                                    + '<div class="title">Current Status</div>'
                                                                    + '<a class="description">' + value.Status + '</a>'
                                                                + '</div>'
                                                                + oldStatusHtml
                                                            + '</div>'
                                                        + '</div>'
                                                        + '<div class="main_card_list_body">'
                                                            + '<div class="row">'
                                                                + '<div class="col-md-12 mt-3">'
                                                                    + '<div class="title">Assign DC Committee</div>'
                                                                    + '<div class="file_name_btn" style="cursor:pointer;" onclick="openDcMemberListView(' + value.Id + ')">'
                                                                        + '<div class="name">' + value.CommitteTitle + '</div>'
                                                                        + '<div class="view_file"><i id="icon_dcMember_' + value.Id + '" class="las la-eye"></i></div>'
                                                                    + '</div>'
                                                                + '</div>'
                                                                + '<input type="hidden" id="' + hdfCommitteeIds + '" value="' + value.CommitteeId + '">'
                                                                + '<input type="hidden" id="' + hdfComplaintIds + '" value="' + value.ComplaintId + '">'
                                                                + '<div><table class="table table-bordered table-sm mt-2" id="' + tblDCMemberDetailsid + '"><thead></thead><tbody></tbody></table></div>'
                                                            + '</div>'
                                                        + '</div>'
                                                     + '</div>'
                                               + '</div>';
                            $('#logDetails').append(logDetailsitem);
                        }
                        else {
                            $('#logDetails').append(
                                '<div class="status_card_details"><div class="check_element"><i class="las la-check-circle"></i></div><div class="main_card_list main_card_list_border main_card_list_border_log w-100"><div class="main_header_card border-0 pb-0"><div class="rigth_side"><div class="user_name_new">' + value.Name + '</div><div class="date_update">' + value.SNextDate + '</div></div><div class="_left_side"><div class="complaint_number paymentstatuspaid"><div class="title">Current Status</div><a class="description">' + value.Status + '</a></div>' + oldStatusHtml + '</div></div></div></div>'
                            );
                        }
                    })
                }
                else {

                }
            }, failure: function (result) {
                swal('Something went wrong!', '', 'error');
            },
            error: function (result) {
                swal('Something went wrong!', '', 'error');
            }
        });
    }

    function GetDcNumberList() {
        $('#drpDcNo').empty();
        $('#drpDcNo').append('<option value="">Select DC Commitee</option>');
        $.ajax({
            type: 'Get',
            url: "/Complaint/GetDcNoList",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.IsSuccess) {
                    $.each(response.Data, function (data, value) {
                        $("#drpDcNo").append('<option value=' + value.Id + '>' + value.DcTitle + '</option>');
                    });
                }
                else {
                    $('#drpDcNo').empty();
                    $('#drpDcNo').append('<option value="">Select DC Commitee</option>');
                }
            }, failure: function (result) {
                $('#drpDcNo').empty();
                $('#drpDcNo').append('<option value="">Select DC Commitee</option>');
                swal('Something went wrong!', '', 'error');
            },
            error: function (result) {
                $('#drpDcNo').empty();
                $('#drpDcNo').append('<option value="">Select DC Commitee</option>');
                swal('Something went wrong!', '', 'error');
            }
        });
    }

    function ddlStatusChange() {
        if ($('#drpStatus option:selected').text() != "Select Status") {
            var status = $('#drpStatus option:selected').val();
            var complaintid = $("#hdfComplaintId").val();
            if (status == "Adjunt") {
                $('#adjuntDetails').css('display', 'block');
                $('#OrderPassed').css('display', 'none');
                $('#boardAccept').css('display', 'none');
                $('#dvOrderPassed').css('display', 'none');
            }
            else if (status == "Order Passed") {
                $('#adjuntDetails').css('display', 'none');
                $('#OrderPassed').css('display', 'block');
                $('#boardAccept').css('display', 'none');
                $('#dvOrderPassed').css('display', 'block');
            }
            else if (status == "Board Has Approved") {
                GetDcNumberList();
                $('#adjuntDetails').css('display', 'none');
                $('#OrderPassed').css('display', 'none');
                $('#boardAccept').css('display', 'block');
                $('#dvOrderPassed').css('display', 'none');
            }
            else {
                $('#adjuntDetails').css('display', 'none');
                $('#OrderPassed').css('display', 'none');
                $('#boardAccept').css('display', 'none');
                $('#dvOrderPassed').css('display', 'none');
                $('#drpStatus').val('');
            }

            var Data = JSON.stringify({
                ComplaintId: $("#hdfComplaintId").val(),
                Status: status
            });

            if (status == 'New' || status == 'Accepted' || status == 'Rejected' || status == 'Rejected' || status == 'More Detail' || status == 'Payment Done' || status == 'Next General Board' || status == 'Board Has Rejected' || status == 'Hearing') {
                $.ajax({
                    type: 'POST',
                    url: "/Complaint/StatusUpdate",
                    contentType: "application/json; charset=utf-8",
                    data: Data,
                    dataType: "json",
                    success: function (response) {
                        GetComplaintCurrentStatus();
                        GetLogList();
                        if (status != "Adjunt") {
                            if (status != "Order Passed") {
                                if (status != "Board Has Approved") {
                                    $('#drpStatus').val('');
                                }
                            }
                        }
                    }, failure: function (result) {
                        swal('Something went wrong!', '', 'error');
                    },
                    error: function (result) {
                        swal('Something went wrong!', '', 'error');
                    }
                });
            }
        }
    }

    function ddlDcNumber() {
        if ($('#drpDcNo option:selected').text() != "Select DC Commitee") {
            var dcno = $('#drpDcNo option:selected').val();
            var Data = JSON.stringify({
                ComplaintId: $("#hdfComplaintId").val(),
                CommitteeId: dcno
            });
            $.ajax({
                type: 'POST',
                url: "/Complaint/GetDcMemberLists",
                contentType: "application/json; charset=utf-8",
                data: Data,
                dataType: "json",
                success: function (response) {
                    $('#tbldcDetails').empty();
                    var dcDetails = $('#tbldcDetails');
                    if (response.IsSuccess) {
                        $.each(response.Data, function (data, value) {
                            dcDetails.append('<div class="main_card_list main_card_list_border mb-2"><div class="main_header_card"><div class="_left_side"><div class="number">' + value.SrNo + '</div><div class="complaint_number"><div class="title">Enrollment Number</div><a class="description">' + value.EnroNumber + '</a></div></div><div class="rigth_side_check"><input type="checkbox" class="main_check_style" name="dcCheck" value="' + value.MemberId + '" checked=""><i class="las la-check"></i></div></div><div class="main_card_list_body"><div class="row"><div class="col-md-6 mt-3"><div class="title">Name</div><div class="description">' + value.FullName + '</div></div><div class="col-md-6 mt-3"><div class="title">Contact Number</div><div class="description">' + value.ContactNumber + '</div></div></div></div></div>')
                        });
                    }
                    else {
                    }
                }, failure: function (result) {
                    swal('Something went wrong!', '', 'error');
                },
                error: function (result) {
                    swal('Something went wrong!', '', 'error');
                }
            });
        }
        else {
            swal("Select Committee", "", "error");
        }
    }

    $('#btnAddToList').on('click', function () {
        var addtoMainList = [];
        addtoMainList.length = 0;
        $.each($("input[name='dcMemberCheck']"), function () {

            var currentRows = $(this).closest(".main_card_list");
            var val = $(this).is(":checked");

            if (val == 1) {
                addtoMainList.push({
                    IsChecked: "1",
                    MemberId: currentRows.find(".number").html(),
                    EnroNumber: currentRows.find(".enrolnumberid").html(),
                    Name: currentRows.find(".nameid").html(),
                    ContactNumber: currentRows.find(".contectid").html()
                });
            }
        });

        var dcDetails = $('#tbldcDetails');

        if (addtoMainList.length > 0) {

            for (var i = 0; i < addtoMainList.length; i++) {
                var id = $('#tbldcDetails > .main_card_list').length;
                var srno = id + 1;
                dcDetails.append('<div class="main_card_list main_card_list_border mb-2"><div class="main_header_card"><div class="_left_side"><div class="number">' + srno + '</div><div class="complaint_number"><div class="title">Enrollment Number</div><a class="description">' + addtoMainList[i].EnroNumber + '</a></div></div><div class="rigth_side_check"><input type="checkbox" class="main_check_style" name="dcCheck" value="' + addtoMainList[i].MemberId + '" checked=""><i class="las la-check"></i></div></div><div class="main_card_list_body"><div class="row"><div class="col-md-6 mt-3"><div class="title">Name</div><div class="description">' + addtoMainList[i].Name + '</div></div><div class="col-md-6 mt-3"><div class="title">Contact Number</div><div class="description">' + addtoMainList[i].ContactNumber + '</div></div></div></div></div>')
            }
        }
        $("#MemberList").modal('hide');
    });

    $('#lnkAddMember').on('click', function () {

        if ($('#drpDcNo option:selected').text() != "Select DC Commitee") {
            var dcno = $('#drpDcNo option:selected').val();

            $.ajax({
                type: 'Get',
                url: "/Complaint/GetDcMemberList",
                contentType: "application/json; charset=utf-8",
                data: { DcNo: dcno },
                dataType: "json",
                success: function (response) {
                    $('#tblMemberList').empty();
                    var dcDetails = $('#tblMemberList');

                    if (response.IsSuccess) {
                        if (response.Data != null) {
                            if (response.Data != '') {
                                $("#MemberList").modal('show');

                                $.each(response.Data, function (data, value) {
                                    dcDetails.append('<div class="main_card_list main_card_list_border mb-3"><div class="main_header_card"><div class="_left_side"><div class="number">' + value.SrNo + '</div><div class="complaint_number"><div class="title">Enrollment Number</div><a class="description enrolnumberid">' + value.EnroNumber + '</a></div></div><div class="rigth_side_check"><input type="checkbox" class="main_check_style" name="dcMemberCheck" value="' + value.MemberId + '"><i class="las la-check"></i></div></div><div class="main_card_list_body"><div class="row"><div class="col-md-6 mt-3"><div class="title">Name</div><div class="description nameid">' + value.FullName + '</div></div><div class="col-md-6 mt-3"><div class="title">Contact Number</div><div class="description contectid">' + value.ContactNumber + '</div></div></div></div></div>')
                                });
                            }
                        }
                    }
                    else {
                        dcDetails.append('No data available');
                    }
                }, failure: function (result) {
                    swal('Something went wrong!', '', 'error');
                },
                error: function (result) {
                    swal('Something went wrong!', '', 'error');
                }
            });
        }
        else {
            swal("Select Committee", "", "error");
        }
    });

    $('#lnkSaveAdjuntDetails').on('click', function () {
        if ($("#txtNextDate").val() != null) {

            if ($("#txtNextDate").val() != "") {
                var Data = JSON.stringify({
                    NextDate: $("#txtNextDate").val(),
                    Summary: $("#txtSummary").val(),
                    ComplaintId: $("#hdfComplaintId").val(),
                    CompalintStatus: $('#drpStatus option:selected').val(),
                    OrderFileName: "",
                    OrderFilePath: "",
                });

                $.ajax({
                    type: 'POST',
                    url: "/Complaint/SaveComplaintStatusLog",
                    data: Data,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.IsSuccess) {
                            $("#txtNextDate").val('')
                            $("#txtSummary").val('');
                            swal("Details save successfully", "", "success");
                            GetComplaintCurrentStatus();
                            GetLogList();
                        }
                        else {
                            swal("Something went wrong", "", "error");
                        }
                    }
                });
            }
            else {
                swal("Please select date", "", "error");
            }
        }
        else {
            swal("Please select date", "", "error");
        }
    });

    $('#lnkSaveBoardAccept').on('click', function () {

        if ($('#drpDcNo option:selected').text() == "Select DC Commitee") {
            swal("Please select DC Commitee", "", "error");
        }
        else if ($('#txtDCNumber').val() == "") {
            swal("Please enter DC Case No.", "", "error");
        }
        else {

            var MemberListArr = [];
            MemberListArr.length = 0;

            $.each($("input[name='dcCheck']"), function () {
                var val = $(this).is(":checked");
                if (val == 1) {
                    MemberListArr.push({
                        MemberId: $(this).val(),
                        IsChecked: "1"
                    });
                }
            });

            var Data = JSON.stringify({
                ComplaintId: $("#hdfComplaintId").val(),
                CompalintStatus: $('#drpStatus option:selected').val(),
                memberList: MemberListArr,
                CommitteeId: $('#drpDcNo option:selected').val(),
                DCReferenceNumber: $("#txtDCNumber").val()
            });

            $.ajax({
                type: 'POST',
                url: "/Complaint/SaveComplaintMemberLog",
                data: Data,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.IsSuccess) {
                        if (response.Message == "IN") {
                            swal("DC Committee Board Has Approved successfully.", "", "success");
                            // Close boardAccept div and clear all fields
                            $('#drpDcNo').empty();
                            $('#drpDcNo').append('<option value="">Select DC Commitee</option>');
                            $('#txtDCNumber').val('');
                            $('#tbldcDetails').empty();
                            $('#boardAccept').css('display', 'none');
                            GetComplaintCurrentStatus();
                            GetLogList();
                            $("#manageStatusModal").modal('hide');
                        }
                        else if (response.Message == "EX") {
                            swal("DC Case No. already exists.", "", "error");
                        }
                        else {
                            swal("Opps! Something Went Wrong.", "", "error");
                        }
                    }
                    else {
                        swal("Opps! Something Went Wrong.", "", "error");
                    }
                }
            });
        }
    });

    $('#lnkCancelBoardAccept').on('click', function () {
        $('#drpDcNo').empty();
        $('#drpDcNo').append('<option value="">Select DC Commitee</option>');
        $('#txtDCNumber').val('');
        $('#tbldcDetails').empty();
        $('#boardAccept').css('display', 'none');
        $('#drpStatus').val('');
    });

    $('#lnkCancelAdjuntDetails').on('click', function () {
        $('#txtNextDate').val('');
        $('#txtSummary').val('');
        $('#adjuntDetails').css('display', 'none');
        $('#drpStatus').val('');
    });

    $('#lnkCancelOrderPassed').on('click', function () {
        $('#orderFile').val('');
        $('#hdfB64ImagePath').val('');
        $('#hdfFileName').val('');
        $('#txtOrderSummary').val('');
        $('#OrderPassed').css('display', 'none');
        $('#drpStatus').val('');
    });

    function ShowPreview(input, txt) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                var PPath = e.target.result;
                if (txt == "Order") {
                    document.getElementById("hdfB64ImagePath").value = PPath
                    document.getElementById("hdfFileName").value = input.files[0].name
                    $("#imgProfileImage").prop('src', e.target.result)
              .css('visibility', 'visible')
                }
                else {
                    document.getElementById("hdfReplyB64ImagePath").value = PPath
                    document.getElementById("hdfReplyFileName").value = input.files[0].name
                    $('#lbldisplayName').text(input.files[0].name);
                }
            };
            reader.readAsDataURL(input.files[0]);
        }
    }

    var FileName;

    $('#lnkSaveOrderDetails').on('click', function () {
        FileName = $("#hdfFileName").val();
        var Data = JSON.stringify({
            NextDate: "",
            Summary: $("#txtOrderSummary").val(),
            ComplaintId: $("#hdfComplaintId").val(),
            CompalintStatus: $('#drpStatus option:selected').val(),
            OrderFileName: $("#hdfFileName").val(),
            OrderFilePath: $('#hdfB64ImagePath').val(),
        });

        $.ajax({
            type: 'POST',
            url: "/Complaint/SaveComplaintStatusLog",
            data: Data,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.IsSuccess) {
                    $("#txtOrderSummary").val('');
                    $('#orderFile').val('');
                    $("#hdfB64ImagePath").val('');
                    $("#hdfFileName").val('');
                    $("#OrderPassed").addClass('hide-element');
                    $("#OrderPassed").hide();
                    swal("Order File save successfully", "", "success");
                    GetComplaintCurrentStatus();
                    GetLogList();
                }
                else {
                    swal("Data already Exists", "", "error");
                }
            }
        });
    });

    function getFile(filepath) {
        if (filepath == "") return;
        var ext = filepath.split('.').pop().toLowerCase();
        var previewExts = ['pdf', 'jpg', 'jpeg', 'png', 'gif', 'bmp', 'svg', 'webp'];
        if (previewExts.indexOf(ext) > -1) {
            $('#displayPdf').attr('src', filepath);
            $('#filePreviewModal').modal('show');
        } else {
            var a = document.createElement('a');
            a.href = filepath;
            a.download = filepath.split('/').pop();
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
        }
    }

    $('#filePreviewModal').on('hidden.bs.modal', function () {
        $('#displayPdf').attr('src', '');
    });

    $('#lnkDownload').on('click', function () {
        var Data = JSON.stringify({
            ComplaintId: $("#hdfComplaintId").val()
        });

        $.ajax({
            type: 'POST',
            url: "/Complaint/GetAllComplaintDetailsReportsFilterSet",
            data: Data,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                if (result.data == "Success") {
                    var url = '@Url.Action("DownloadDetailReport")';
                    location.href = url;
                } else {
                    swal('Something went wrong!', '', 'error');
                }
            }
        });
    });

    function GetReplyList(id) {
        $('#hdfSenderId').val(id);
        var Data = JSON.stringify({
            SenderId: 0,
            ComplaintId: $("#hdfComplaintId").val(),
        });

        $.ajax({
            type: "POST",
            url: "/Complaint/GetReplyList",
            data: Data,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                $('#dvreplyData').empty();
                var dcReply = $('#dvreplyData');
                if (response.IsSuccess) {
                    $.each(response.Data, function (data, row) {
                        var FilePath = "'" + row.FilePath + "'";
                        var deleteBtnHtml = '';
                        if (row.IsShowDeleteButton === 'Y') {
                            deleteBtnHtml = '<button type="button" class="btn btn-outline-danger btn-sm ms-2 py-0" style="font-size: 11px; line-height: 1.2;" onclick="deleteReply(\'' + row.Id + '\')"><i class="fa-solid fa-trash me-1"></i>Delete</button>';
                        }
                        var RowItem = '<div class="replaydetailsmain">'
                                        + '<div class="Sub_titile d-flex justify-content-between sendernamestyle">'
                                            + '<div class="fw-bold font-size-17 text-dark"><i class="fa-solid fa-user text-muted me-2"></i>' + row.SenderName + '</div>'
                                            + '<div class="d-flex align-items-center">'
                                                + '<span class="fw-bold font-size-17 text-dark"><i class="fa-regular fa-clock text-muted me-2"></i>' + row.SCreateDate + ' ' + row.SCreateTime + '</span>'
                                                + deleteBtnHtml
                                            + '</div>'
                                        + '</div>';

                        if (row.Summery != null && row.Summery.trim() !== "") {
                            RowItem += '<div class="Sub_titile mt-2">'
                                             + 'Summery | Short Information'
                                        + '</div>'
                                        + '<div class="main_contentupdate">'
                                             + '<p>' + row.Summery + '</p>'
                                        + '</div>';
                        }

                        if (row.Comprehensive != null && row.Comprehensive.trim() !== "") {
                            RowItem += '<div class="Sub_titile mt-3">'
                                             + 'Comprehensive Complaint'
                                        + '</div>'
                                        + '<div class="main_contentupdate">'
                                             + '<p>' + row.Comprehensive + '</p>'
                                        + '</div>';
                        }

                        if (row.FilePath != null && row.FilePath.trim() !== "") {
                            RowItem += '<div class="Sub_titile mt-3">Attachment</div>'
                                            + '<div class="main_contentupdate">'
                                                + '<div class="fileupload_details">'
                                                    + '<div class="d-flex align-items-center" onclick="getFile(' + FilePath + ')" style="cursor:pointer;" title="Click to view file">'
                                                        + '<div class="pdf-btn btn-sm">'
                                                            + '<img src="/assets/img/pdficon.svg">'
                                                        + '</div>'
                                                        + '<div class="fileupload_name text-primary text-decoration-underline">' + row.FileName + '</div>'
                                                    + '</div>'
                                                + '</div>'
                                            + '</div>';
                        }

                        RowItem += '</div>';
                        dcReply.prepend(RowItem);
                    });
                    dcReply.scrollTop(0);
                }
                else {
                    dcReply.append('No data available');
                }
            }, failure: function (result) {
                swal('Something went wrong!', '', 'error');
            },
            error: function (result) {
                swal('Something went wrong!', '', 'error');
            }
        });
    }

    function deleteReply(id) {
        Swal.fire({
            title: 'Are you sure?',
            text: "You want to delete this reply?",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.isConfirmed) {
                var data = {
                    Id: id
                };

                $.ajax({
                    type: 'POST',
                    url: "/Complaint/ComplaintReplyDelete",
                    data: JSON.stringify(data),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.IsSuccess) {
                            Swal.fire("Deleted!", "Reply deleted successfully.", "success");
                            GetReplyList($('#hdfSenderId').val());
                        } else {
                            Swal.fire("Failed!", response.Message || "Failed to delete reply.", "error");
                        }
                    },
                    error: function (result) {
                        Swal.fire("Error!", "Error deleting reply.", "error");
                    }
                });
            }
        });
    }

    $(document).on('click', '#lnkManageStatus', function () {
        GetLogList();
        $('#manageStatusModal').modal('show');
    });

    $('#btnReplySave').on('click', function () {

        var replyMessage = $('#txtReply').val() ? $('#txtReply').val().trim() : "";
        var fileName = $("#hdfReplyFileName").val() ? $("#hdfReplyFileName").val().trim() : "";

        if (replyMessage === "" && fileName === "") {
            swal("Please enter a message or upload a file.", "", "error");
            return false;
        }

        var Data = {
            ComplaintId: $("#hdfComplaintId").val(),
            ReplyMessage: $('#txtReply').val(),
            FileName: $("#hdfReplyFileName").val(),
            FilePath: $('#hdfReplyB64ImagePath').val(),
        };

        $.ajax({
            type: "POST",
            url: "/Complaint/InsertComplaintReply",
            data: Data,
            dataType: "json",
            success: function (response) {
                if (response.IsSuccess) {
                    swal('Reply Sent Successfully.', '', 'success');
                    $('#txtReply').val('');
                    $('#lbldisplayName').text('');
                    GetReplyList($('#hdfSenderId').val());
                }
                else {
                    swal('Something went wrong!', '', 'error');
                }
            }, failure: function (result) {
                swal('Something went wrong!', '', 'error');
            },
            error: function (result) {
                swal('Something went wrong!', '', 'error');
            }
        });
    });

    function accordian_new(element) {
        var $element = $(element);
        if ($element.hasClass("active")) {
            $element.removeClass("active").parent().parent().next(".answer").slideUp();
        } else {
            $(".main_btn_acc.active").removeClass("active").parent().parent().next(".answer").slideUp();
            $element.addClass("active").parent().parent().next(".answer").slideDown();
        }
        return false;
    }

    function openComplainantEditModal() {
        $('#txtEditFullName').val($('#lblComplainantFullName').text().trim());
        $('#txtEditEmail').val($('#lblComplainantEmail').text().trim());
        $('#txtEditContact').val($('#lblComplainantContact').text().trim());
        $('#txtEditAddress').val($('#lblComplainantAddress').text().trim());
        $('#editComplainantModal').modal('show');
    }

    function saveComplainantDetails() {
        var data = {
            Id: $('#hdfComplaintId').val(),
            FullName: $('#txtEditFullName').val(),
            EmailAddress: $('#txtEditEmail').val(),
            ContactNumber: $('#txtEditContact').val(),
            Address: $('#txtEditAddress').val()
        };

        $.ajax({
            type: "POST",
            url: "/Complaint/UpdateComplainantDetails",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.IsSuccess) {
                    $('#editComplainantModal').modal('hide');
                    swal(response.Message, '', 'success');
                    ComplaintComplainantsDetailsByComplaintNumberRefresh();
                } else {
                    swal(response.Message, '', 'error');
                }
            },
            error: function (result) {
                swal('An error occurred while updating details.', '', 'error');
            }
        });
    }

    function openAffidavitUploadModal() {
        $('#fileAffidavitCopy').val('');
        $('#uploadAffidavitModal').modal('show');
    }

    function saveAffidavitCopy() {
        var fileUpload = $("#fileAffidavitCopy").get(0);
        var files = fileUpload.files;

        if (files.length === 0) {
            swal("Please select a file to upload.", "", "error");
            return;
        }

        var file = files[0];
        var reader = new FileReader();

        reader.onload = function (e) {
            var base64File = e.target.result;

            var data = {
                ComplaintId: $('#hdfComplaintId').val(),
                FileName: file.name,
                Base64File: base64File
            };

            $.ajax({
                type: "POST",
                url: "/Complaint/UploadAffidavitCopy",
                data: data,
                dataType: "json",
                success: function (response) {
                    if (response.IsSuccess) {
                        GetComplaintAffidavitCopyList();
                        $('#uploadAffidavitModal').modal('hide');
                        $('#fileAffidavitCopy').val('');
                        swal(response.Message, '', 'success');
                    } else {
                        swal(response.Message, '', 'error');
                    }
                },
                error: function (result) {
                    swal('An error occurred while uploading the file.', '', 'error');
                }
            });
        };

        reader.readAsDataURL(file);
    }

    function GetComplaintAffidavitCopyList() {

        var numberPart = $('#txtSearchComplaintNumber').val().trim();
        var yearPart = $('#drpSearchComplaintYear').val().trim();

        if (numberPart === '' || yearPart === '') {
            swal('Please enter Complaint Number and select Year.', '', 'error');
            return;
        }

        var fullComplaintNumber = 'BCG/CMP/' + numberPart + '/' + yearPart;

        var Data = JSON.stringify({
            ComplaintNumber: fullComplaintNumber
        });

        $.ajax({
            type: "POST",
            url: "/Complaint/GetComplaintAffidavitCopyLog",
            contentType: "application/json; charset=utf-8",
            data: Data,
            dataType: "json",
            success: function (response) {
                $("#dvAffidavitCopy").empty();
                if (response.IsSuccess) {
                    if (response.Data != null) {
                        if (response.Data != '') {
                            $.each(response.Data, function (i, item) {
                                if (item.FilePath != null && item.FilePath !== '') {
                                    var affHtml = '<div class="main_card_list main_card_list_border p-12-custome mt-3">'
                                        + '<div class="main_card_list_body"><div class="row"><div class="col-md-12">'
                                        + '<div class="file_name_btn" onclick="getFile(\'' + item.FilePath + '\')"><div class="name">' + (item.FileName || '') + '</div><div class="view_file"><i class="las la-eye"></i></div></div>'
                                        + '</div></div></div></div>';
                                    $('#dvAffidavitCopy').append(affHtml);
                                }
                            });
                        }
                        else {
                            $("#dvAffidavitCopy").empty();
                        }
                    }
                    else {
                        $("#dvAffidavitCopy").empty();
                    }
                }
                else {
                    $("#dvAffidavitCopy").empty();
                }
            }, failure: function (result) {
                $("#dvAffidavitCopy").empty();
            },
            error: function (result) {
                $("#dvAffidavitCopy").empty();
            }
        });
    }

    function openAdditionalAttachmentModal() {
        $('#fileAdditionalAttachment').val('');
        $('#txtAdditionalFileDescription').val('');
        $('#uploadAdditionalAttachmentModal').modal('show');
    }

    function saveAdditionalAttachment() {
        var fileUpload = $("#fileAdditionalAttachment").get(0);
        var description = $('#txtAdditionalFileDescription').val();
        var files = fileUpload.files;

        if (files.length === 0) {
            swal("Please select a file to upload.", "", "error");
            return;
        }

        var file = files[0];
        var reader = new FileReader();

        reader.onload = function (e) {
            var base64File = e.target.result;

            var data = {
                ComplaintId: $('#hdfComplaintId').val(),
                FileTitle: '',
                FileName: file.name,
                Base64File: base64File,
                Description: description
            };

            $.ajax({
                type: "POST",
                url: "/Complaint/UploadAdditionalAttachment",
                data: data,
                dataType: "json",
                success: function (response) {
                    if (response.IsSuccess) {
                        GetComplaintAdditionalAttachmentsList();
                        $('#uploadAdditionalAttachmentModal').modal('hide');
                        $('#fileAdditionalAttachment').val('');
                        swal(response.Message, '', 'success');
                    } else {
                        swal(response.Message, '', 'error');
                    }
                },
                error: function (result) {
                    swal('An error occurred while uploading the attachment.', '', 'error');
                }
            });
        };

        reader.readAsDataURL(file);
    }

    function GetComplaintAdditionalAttachmentsList() {

        var numberPart = $('#txtSearchComplaintNumber').val().trim();
        var yearPart = $('#drpSearchComplaintYear').val().trim();

        if (numberPart === '' || yearPart === '') {
            swal('Please enter Complaint Number and select Year.', '', 'error');
            return;
        }

        var fullComplaintNumber = 'BCG/CMP/' + numberPart + '/' + yearPart;

        var Data = JSON.stringify({
            ComplaintNumber: fullComplaintNumber
        });

        $.ajax({
            type: "POST",
            url: "/Complaint/GetComplaintAdditionalAttachmentsLog",
            contentType: "application/json; charset=utf-8",
            data: Data,
            dataType: "json",
            success: function (response) {
                $("#dvOrderPassed").empty();
                if (response.IsSuccess) {
                    if (response.Data != null) {
                        if (response.Data != '') {
                            $.each(response.Data, function (i, item) {
                                var fileBlock = '';
                                if (item.FilePath != null && item.FilePath !== '') {
                                    fileBlock = '<div class="d-flex align-items-center">'
                                        + '<div class="file_name_btn flex-grow-1" onclick="getFile(\'' + item.FilePath + '\')"><div class="name">' + (item.FileName || '') + '</div><div class="view_file"><i class="las la-eye"></i></div></div>'
                                        + '<a href="javascript:void(0);" class="btn btn-outline-danger btn-sm ms-2" style="height: 35px; display: flex; align-items: center; justify-content: center;" onclick="deleteAdditionalAttachment(' + item.Id + ')" title="Delete File"><i class="las la-trash fs-5"></i></a>'
                                        + '</div>';
                                }
                                var noteBlock = '';
                                if (item.Description != null && item.Description.trim() !== '') {
                                    noteBlock = '<div class="col-md-12 mt-3"><div class="title">Note (નોંધ)</div><div class="description">' + item.Description + '</div></div>';
                                }
                                var addHtml = '<div class="main_card_list main_card_list_border p-12-custome mt-3">'
                                    + '<div class="main_card_list_body"><div class="row">'
                                    + '<div class="col-md-12"><div class="title">Upload File (ફાઇલ અપલોડ)</div>' + fileBlock + '</div>'
                                    + noteBlock
                                    + '</div></div></div>';
                                $('#dvOrderPassed').append(addHtml);
                            });
                        }
                        else {
                            $("#dvOrderPassed").empty();
                        }
                    }
                    else {
                        $("#dvOrderPassed").empty();
                    }
                }
                else {
                    $("#dvOrderPassed").empty();
                }
            }, failure: function (result) {
                $("#dvOrderPassed").empty();
            },
            error: function (result) {
                $("#dvOrderPassed").empty();
            }
        });
    }

    function deleteAdditionalAttachment(id) {
        if (confirm("Are you sure you want to delete this attachment?")) {
            $.ajax({
                type: "POST",
                url: "/Complaint/DeleteAdditionalAttachment",
                data: JSON.stringify({ Id: id }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.IsSuccess) {
                        swal(response.Message, '', 'success');
                        location.reload();
                    } else {
                        swal(response.Message, '', 'error');
                    }
                },
                error: function (result) {
                    swal('An error occurred while deleting the attachment.', '', 'error');
                }
            });
        }
    }

    function openAddMemberModal() {
        $('#txtNewEnrollmentNumber').val('');
        $('#hdfSelectedEnrollmentNumber').val('');
        $('#advocateSearchResults').hide().empty();
        $('#addMemberModal').modal('show');
    }

    // Automatic stacking of multiple Bootstrap modals
    $(document).on('show.bs.modal', '.modal', function () {
        var zIndex = 1050 + (10 * $('.modal.show').length);
        $(this).css('z-index', zIndex);
        setTimeout(function () {
            $('.modal-backdrop').not('.modal-stack').css('z-index', zIndex - 1).addClass('modal-stack');
        }, 0);
    });

    $(document).on('hidden.bs.modal', '.modal', function () {
        $(this).css('z-index', '');
        $('.modal.show').each(function (index) {
            var zIndex = 1050 + (10 * index);
            $(this).css('z-index', zIndex);
        });
        $('.modal-backdrop').each(function (index) {
            var zIndex = 1049 + (10 * index);
            $(this).css('z-index', zIndex);
        });
        if ($('.modal.show').length > 0) {
            setTimeout(function () {
                $(document.body).addClass('modal-open');
            }, 0);
        }
    });

    $('#txtNewEnrollmentNumber').on('input', function () {
        clearTimeout(searchTimeout);
        var searchTitle = $(this).val().trim();
        $('#hdfSelectedEnrollmentNumber').val('');

        if (searchTitle.length < 1) {
            $('#advocateSearchResults').hide().empty();
            detailsSearchPageNo = 1;
            detailsHasMore = true;
            detailsCurrentSearch = "";
            return;
        }

        // Reset pagination on new search
        detailsCurrentSearch = searchTitle;
        detailsSearchPageNo = 1;
        detailsHasMore = true;
        $('#advocateSearchResults').empty();

        searchTimeout = setTimeout(function () {
            loadDetailsSearchData(searchTitle, true);
        }, 500);
    });

    $('#advocateSearchResults').on('scroll', function () {
        var container = $(this);
        if (container.scrollTop() + container.innerHeight() >= container[0].scrollHeight - 20) {
            if (!detailsIsLoading && detailsHasMore && detailsCurrentSearch !== "") {
                detailsSearchPageNo++;
                loadDetailsSearchData(detailsCurrentSearch, false);
            }
        }
    });

    function loadDetailsSearchData(searchTitle, isNewSearch) {
        if (detailsIsLoading) return;
        detailsIsLoading = true;

        var $results = $('#advocateSearchResults');
        if (isNewSearch) {
            $results.empty();
        }
        $results.append('<li id="detailsSearchLoader" class="list-group-item text-center"><i class="fa-solid fa-spinner fa-spin"></i> Loading...</li>');
        $results.show();

        $.ajax({
            type: 'POST',
            url: '/Complaint/GetSearchData',
            data: JSON.stringify({ SearchTitle: searchTitle, PageNo: detailsSearchPageNo, PageSize: detailsSearchPageSize }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {
                $('#detailsSearchLoader').remove();
                detailsIsLoading = false;

                if (response && response.data && response.data.length > 0) {
                    $.each(response.data, function (index, item) {
                        var displayText = item.FullName + ' (' + item.EnroNumber + ') - ' + (item.ContactNumber || '');
                        var $li = $('<li class="list-group-item list-group-item-action" style="cursor: pointer;"></li>')
                            .text(displayText)
                            .on('click', function () {
                                $('#txtNewEnrollmentNumber').val(displayText);
                                $('#hdfSelectedEnrollmentNumber').val(item.EnroNumber);
                                $results.hide();
                            });
                        $results.append($li);
                    });
                    $results.show();

                    // Check if more data exists
                    var totalRecord = response.totalRecord || 0;
                    var totalLoaded = $results.find('.list-group-item-action').length;
                    if (totalLoaded >= totalRecord || response.data.length < detailsSearchPageSize) {
                        detailsHasMore = false;
                    }
                } else {
                    detailsHasMore = false;
                    if (isNewSearch) {
                        $results.append('<li class="list-group-item text-muted">No advocates found</li>');
                        $results.show();
                    }
                }
            },
            error: function () {
                $('#detailsSearchLoader').remove();
                detailsIsLoading = false;
                if (isNewSearch) {
                    $results.hide().empty();
                }
            }
        });
    }

    function saveNewMember() {
        var enrollmentNo = $('#hdfSelectedEnrollmentNumber').val().trim();
        if (enrollmentNo === "") {
            swal("Please select an Advocate from the search results.", "", "error");
            return;
        }

        var complaintId = $("#hdfComplaintId").val();

        var data = {
            Id: complaintId,
            EnrollmentNumber: enrollmentNo
        };

        $.ajax({
            type: 'POST',
            url: "/Complaint/ComplaintAgainstSave",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.IsSuccess) {
                    swal("Member added successfully!", "", "success");
                    $('#addMemberModal').modal('hide');
                    $('#selectDetails').empty();
                    GetDefendantList();
                    GetComplaintPaymentDetail();
                } else {
                    swal("Failed to add member. Please check the Enrollment Number.", "", "error");
                }
            },
            error: function (result) {
                swal("Error adding member.", "", "error");
            }
        });
    }

    function openComplaintGBMeetingModal() {
        $('#hdfGBMeetingId').val('0');
        $('#txtMeetingTitle').val('');
        $('#txtMeetingDate').val('');
        $('#txtMeetingNote').val('');
        $('#mbComplaintGBMeeting').modal('show');
    }

    function getGBMeetingLogs(complaintId) {
        if (!complaintId || complaintId === "0") {
            $('#dvGBMeetingList').empty();
            return;
        }
        $.ajax({
            type: 'POST',
            url: "/Complaint/GetComplaintGBMeetingLogList",
            data: { ComplaintId: complaintId },
            dataType: "json",
            success: function (response) {
                var $container = $('#dvGBMeetingList');
                $container.empty();
                if (response.IsSuccess && response.Data && response.Data.length > 0) {
                    $.each(response.Data, function (i, item) {
                        var html = '<div class="main_card_list main_card_list_border p-12-custome mt-3">'
                            + '<div class="main_card_list_body">'
                                + '<div class="row">'
                                    + '<div class="col-md-4">'
                                        + '<div class="title">Meeting Date (બેઠકની તારીખ)</div>'
                                        + '<div class="description meeting-date-content">' + (item.MeetingDate || '') + '</div>'
                                    + '</div>'
                                    + '<div class="col-md-8">'
                                        + '<div class="title">Meeting Title (અપલોડ શીર્ષક)</div>'
                                        + '<div class="description meeting-title-content">' + (item.MeetingTitle || '') + '</div>'
                                    + '</div>'
                                    + '<div class="col-md-12 mt-3">'
                                        + '<div class="title d-flex justify-content-between align-items-center">'
                                            + '<span>Meeting Note (બેઠકની નોંધ)</span>'
                                            + '<div>'
                                                + '<button type="button" class="btn btn-outline-danger btn-sm px-2 py-0 me-1" style="font-size: 11px; line-height: 1.5;" onclick="deleteGBMeetingLog(\'' + item.Id + '\')"><i class="fa-solid fa-trash me-1"></i>Delete</button>'
                                                + '<button type="button" class="btn btn-outline-info btn-sm px-2 py-0 me-1" style="font-size: 11px; line-height: 1.5;" onclick="editGBMeetingLog(\'' + item.Id + '\', this)"><i class="fa-solid fa-pen-to-square me-1"></i>Edit</button>'
                                                + '<button type="button" class="btn btn-outline-primary btn-sm px-2 py-0" style="font-size: 11px; line-height: 1.5;" onclick="viewMeetingNote(this)"><i class="fa-solid fa-eye me-1"></i>View Note</button>'
                                            + '</div>'
                                        + '</div>'
                                        + '<div class="description meeting-note-content">' + (item.MeetingNote || '') + '</div>'
                                    + '</div>'
                                + '</div>'
                            + '</div>'
                        + '</div>';
                        $container.append(html);
                    });
                }
            },
            error: function () {
                $('#dvGBMeetingList').empty();
            }
        });
    }

    function saveGBMeetingLog() {
        var id = $('#hdfGBMeetingId').val() || "0";
        var complaintId = $("#hdfComplaintId").val();
        var meetingTitle = $('#txtMeetingTitle').val().trim();
        var meetingDate = $('#txtMeetingDate').val().trim();
        var meetingNote = $('#txtMeetingNote').val().trim();

        if (meetingTitle === "") {
            swal("Please enter Meeting Title.", "", "error");
            return;
        }
        if (meetingDate === "") {
            swal("Please select Meeting Date.", "", "error");
            return;
        }

        var data = {
            Id: id,
            ComplaintId: complaintId,
            MeetingTitle: meetingTitle,
            MeetingDate: meetingDate,
            MeetingNote: meetingNote,
            ActionMode: id !== "0" ? "Update" : "Save"
        };

        $.ajax({
            type: 'POST',
            url: "/Complaint/ComplaintGBMeetingLogSave",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.IsSuccess) {
                    var returnStatus = (response.Data && response.Data[0] && response.Data[0].ReturnStatus) ? response.Data[0].ReturnStatus : '';
                    if (returnStatus === 'UD') {
                        swal("GB Meeting Details updated successfully!", "", "success");
                    } else {
                        swal("GB Meeting Details saved successfully!", "", "success");
                    }
                    $('#mbComplaintGBMeeting').modal('hide');
                    getGBMeetingLogs(complaintId);
                } else {
                    swal("Failed to save GB Meeting Details.", "", "error");
                }
            },
            error: function (result) {
                swal("Error saving GB Meeting Details.", "", "error");
            }
        });
    }

    function viewMeetingNote(btn) {
        var noteText = $(btn).closest('.col-md-12').find('.meeting-note-content').text();
        $('#lblViewMeetingNoteText').text(noteText);
        $('#mbViewMeetingNote').modal('show');
    }

    function formatDateForInput(dateStr) {
        if (!dateStr) return '';
        var d = new Date(dateStr);
        if (!isNaN(d.getTime())) {
            var day = ("0" + d.getDate()).slice(-2);
            var month = ("0" + (d.getMonth() + 1)).slice(-2);
            return d.getFullYear() + "-" + month + "-" + day;
        }

        var parts = dateStr.split(/[\/\-\s]/);
        if (parts.length >= 3) {
            var day = parts[0];
            var month = parts[1];
            var year = parts[2];
            var months = ["jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec"];
            var monthLower = month.toLowerCase();
            for (var i = 0; i < months.length; i++) {
                if (monthLower.indexOf(months[i]) === 0) {
                    month = i + 1;
                    break;
                }
            }
            month = ("0" + month).slice(-2);
            day = ("0" + day).slice(-2);
            if (year.length === 2) {
                year = "20" + year;
            }
            if (year.length === 4) {
                return year + "-" + month + "-" + day;
            }
        }
        return '';
    }

    function editGBMeetingLog(id, btn) {
        var $card = $(btn).closest('.main_card_list_body');
        var title = $card.find('.meeting-title-content').text().trim();
        var dateStr = $card.find('.meeting-date-content').text().trim();
        var noteText = $card.find('.meeting-note-content').text();

        $('#hdfGBMeetingId').val(id);
        $('#txtMeetingTitle').val(title);
        $('#txtMeetingDate').val(formatDateForInput(dateStr));
        $('#txtMeetingNote').val(noteText);
        $('#mbComplaintGBMeeting').modal('show');
    }

    function deleteGBMeetingLog(id) {
        Swal.fire({
            title: 'Are you sure?',
            text: "You want to delete this GB Meeting log?",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.isConfirmed) {
                var complaintId = $("#hdfComplaintId").val();
                var data = {
                    Id: id,
                    ComplaintId: complaintId,
                    MeetingTitle: "",
                    MeetingDate: "",
                    MeetingNote: "",
                    ActionMode: "DeleteUpdate"
                };

                $.ajax({
                    type: 'POST',
                    url: "/Complaint/ComplaintGBMeetingLogSave",
                    data: JSON.stringify(data),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.IsSuccess) {
                            var returnStatus = (response.Data && response.Data[0] && response.Data[0].ReturnStatus) ? response.Data[0].ReturnStatus : '';
                            if (returnStatus === 'UD') {
                                Swal.fire("Deleted!", "GB Meeting Details deleted successfully.", "success");
                            } else {
                                Swal.fire("Deleted!", "GB Meeting Details deleted.", "success");
                            }
                            getGBMeetingLogs(complaintId);
                        } else {
                            Swal.fire("Failed!", "Failed to delete GB Meeting Details.", "error");
                        }
                    },
                    error: function (result) {
                        Swal.fire("Error!", "Error deleting GB Meeting Details.", "error");
                    }
                });
            }
        });

    function updateActionButtonsUI(status) {
        var html = '';
        if (status == 'New' || status == 'Old' ||  status == 'More Detail' || status == 'Payment Done') {
            html = '<a href="javascript:void(0);" class="btn btn-success waves-effect waves-light white-space-nowrap btn-sm me-2" onclick="updateStatusDirectly(\'Accepted\')"><i class="fa-solid fa-check me-1"></i> Accept</a>' +
                   '<a href="javascript:void(0);" class="btn btn-warning waves-effect waves-light white-space-nowrap btn-sm me-2" onclick="updateStatusDirectly(\'More Detail\')"><i class="fa-solid fa-info-circle me-1"></i> More Detail</a>' +
                   '<a href="javascript:void(0);" class="btn btn-danger waves-effect waves-light white-space-nowrap btn-sm me-2" onclick="updateStatusDirectly(\'Rejected\')"><i class="fa-solid fa-times me-1"></i> Reject</a>' +
                   '<a href="javascript:void(0);" class="btn btn-secondary waves-effect waves-light white-space-nowrap btn-sm" onclick="updateStatusWithNote(\'Withdrawal Granted\')"><i class="fa-solid fa-arrow-right-from-bracket me-1"></i> Withdrawal Granted</a>';
        } else if (status == 'Accepted') {
            html = '<a href="javascript:void(0);" class="btn btn-info waves-effect waves-light white-space-nowrap btn-sm" onclick="updateStatusDirectly(\'Next General Board\')"><i class="fa-solid fa-gavel me-1"></i> Next General Board</a>';
        } else if (status == 'Next General Board') {
            html = '<a href="javascript:void(0);" class="btn btn-success waves-effect waves-light white-space-nowrap btn-sm me-2" onclick="triggerBoardHasApproved()"><i class="fa-solid fa-check-double me-1"></i> Board Has Approved</a>' +
                   '<a href="javascript:void(0);" class="btn btn-danger waves-effect waves-light white-space-nowrap btn-sm" onclick="updateStatusDirectly(\'Board Has Rejected\')"><i class="fa-solid fa-ban me-1"></i> Board Has Rejected</a>';
        }

        $('#divActionButtons').html(html);
        if (html == '') {
            $('#divActionButtons').hide();
        } else {
            $('#divActionButtons').show();
        }
    }

    function triggerBoardHasApproved() {
        // Move the #boardAccept div into the small standalone modal popup
        $('#boardAcceptOnlyModalBody').append($('#boardAccept'));
        $('#boardAccept').show(); // ensure it's visible inside the new modal

        // Show the new modal popup
        $('#boardAcceptOnlyModal').modal('show');

        // Update the dropdown values in the background for consistency
    }

    function updateStatusDirectly(status) {
        Swal.fire({
            title: "Are you sure?",
            text: "You want to change the status to " + status + "?",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: "Yes",
            cancelButtonText: "No"
        }).then((result) => {
            if (result.isConfirmed) {
                var Data = JSON.stringify({
                    ComplaintId: $("#hdfComplaintId").val(),
                    Status: status
                });

                $.ajax({
                    type: 'POST',
                    url: "/Complaint/StatusUpdate",
                    contentType: "application/json; charset=utf-8",
                    data: Data,
                    dataType: "json",
                    success: function (response) {
                        if (response.IsSuccess) {
                            Swal.fire("Success!", "Status updated successfully.", "success");
                            GetComplaintCurrentStatus();
                            GetLogList();
                        } else {
                            Swal.fire("Error!", response.Message || "Something went wrong!", "error");
                        }
                    },
                    failure: function (result) {
                        Swal.fire('Something went wrong!', '', 'error');
                    },
                    error: function (result) {
                        Swal.fire('Something went wrong!', '', 'error');
                    }
                });
            }
        });
    }

    function updateStatusWithNote(status) {
        Swal.fire({
            title: status,
            text: "Please enter a note for this action:",
            icon: "info",
            input: 'textarea',
            inputPlaceholder: 'Enter your note here...',
            inputAttributes: {
                'aria-label': 'Enter your note here'
            },
            showCancelButton: true,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: "Submit",
            cancelButtonText: "Cancel",
            preConfirm: (note) => {
                if (!note || note.trim() === "") {
                    Swal.showValidationMessage('A note is required to proceed.');
                }
                return note;
            }
        }).then((result) => {
            if (result.isConfirmed) {
                var note = result.value;
                var Data = JSON.stringify({
                    NextDate: "",
                    Summary: note,
                    ComplaintId: $("#hdfComplaintId").val(),
                    CompalintStatus: status,
                    OrderFileName: "",
                    OrderFilePath: ""
                });

                $.ajax({
                    type: 'POST',
                    url: "/Complaint/SaveComplaintStatusLog",
                    contentType: "application/json; charset=utf-8",
                    data: Data,
                    dataType: "json",
                    success: function (response) {
                        if (response.IsSuccess) {
                            Swal.fire("Success!", "Status updated successfully.", "success");
                            GetComplaintCurrentStatus();
                            GetLogList();
                        } else {
                            Swal.fire("Error!", response.Message || "Something went wrong!", "error");
                        }
                    },
                    error: function () {
                        Swal.fire('Something went wrong!', '', 'error');
                    }
                });
            }
        });
    }

