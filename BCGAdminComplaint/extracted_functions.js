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
                    url: "/ComplaintDetails/StatusUpdate",
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
                    url: "/ComplaintDetails/SaveComplaintStatusLog",
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
