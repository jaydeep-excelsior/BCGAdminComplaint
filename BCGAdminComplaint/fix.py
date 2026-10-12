import re

with open('d:/Project/ASP .Net Project/BCG/Complaint/BCGAdminComplaint/BCGAdminComplaint/Views/ComplaintDetails/DetailsNew.cshtml', 'r', encoding='utf-8') as f:
    content = f.read()

pattern = re.compile(r"\+ '<div class=\"col-md-12 mt-3\">'\s*error: function \(\) \{\s*\$\('#dvGBMeetingList'\)\.empty\(\);\s*\}\s*\}\);\s*\}\s*function updateActionButtonsUI\(status\) \{\s*// Implementation for updating buttons based on status\s*console\.log\(\"Status updated to: \" \+ status\);\s*\}\s*\$\(document\)\.ready\(function \(\) \{\s*// Init Action buttons on load based on ViewBag\.Status\s*updateActionButtonsUI\('@ViewBag\.Status'\);\s*\}\);\s*</script>", re.DOTALL)

replacement = """+ '<div class="col-md-12 mt-3">'
                                        + '<div class="title d-flex justify-content-between align-items-center">'
                                            + '<span>Meeting Note (બેઠકની નોંધ)</span>'
                                            + '<div>'
                                                + '<button type="button" class="btn btn-outline-danger btn-sm px-2 py-0 me-1" style="font-size: 11px; line-height: 1.5;" onclick="deleteGBMeetingLog(\\'' + item.Id + '\\')"><i class="fa-solid fa-trash me-1"></i>Delete</button>'
                                                + '<button type="button" class="btn btn-outline-info btn-sm px-2 py-0 me-1" style="font-size: 11px; line-height: 1.5;" onclick="editGBMeetingLog(\\'' + item.Id + '\\', this)"><i class="fa-solid fa-pen-to-square me-1"></i>Edit</button>'
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

    $(document).ready(function () {
        // Init Action buttons on load based on ViewBag.Status
        updateActionButtonsUI('@ViewBag.Status');
    });
</script>"""

new_content = pattern.sub(replacement, content)

with open('d:/Project/ASP .Net Project/BCG/Complaint/BCGAdminComplaint/BCGAdminComplaint/Views/ComplaintDetails/DetailsNew.cshtml', 'w', encoding='utf-8') as f:
    f.write(new_content)
