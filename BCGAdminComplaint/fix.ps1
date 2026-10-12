$content = Get-Content -Path 'd:\Project\ASP .Net Project\BCG\Complaint\BCGAdminComplaint\BCGAdminComplaint\Views\ComplaintDetails\DetailsNew.cshtml' -Raw
$pattern = "\+ '<div class=`"col-md-12 mt-3`">'(?s).*?error: function \(\) \{(?s).*?\}\);(?s).*?\}(?s).*?function updateActionButtonsUI\(status\) \{(?s).*?console\.log\(`"Status updated to: `" \+ status\);(?s).*?\}(?s).*?\$\(document\)\.ready\(function \(\) \{(?s).*?updateActionButtonsUI\('@ViewBag\.Status'\);(?s).*?\}\);\s*</script>"
$replacement = "+ '<div class=`"col-md-12 mt-3`">`n" +
"                                        + '<div class=`"title d-flex justify-content-between align-items-center`">`n" +
"                                            + '<span>Meeting Note (બેઠકની નોંધ)</span>'`n" +
"                                            + '<div>'`n" +
"                                                + '<button type=`"button`" class=`"btn btn-outline-danger btn-sm px-2 py-0 me-1`" style=`"font-size: 11px; line-height: 1.5;`" onclick=`"deleteGBMeetingLog(\'' + item.Id + '\')`"><i class=`"fa-solid fa-trash me-1`"></i>Delete</button>'`n" +
"                                                + '<button type=`"button`" class=`"btn btn-outline-info btn-sm px-2 py-0 me-1`" style=`"font-size: 11px; line-height: 1.5;`" onclick=`"editGBMeetingLog(\'' + item.Id + '\', this)`"><i class=`"fa-solid fa-pen-to-square me-1`"></i>Edit</button>'`n" +
"                                                + '<button type=`"button`" class=`"btn btn-outline-primary btn-sm px-2 py-0`" style=`"font-size: 11px; line-height: 1.5;`" onclick=`"viewMeetingNote(this)`"><i class=`"fa-solid fa-eye me-1`"></i>View Note</button>'`n" +
"                                            + '</div>'`n" +
"                                        + '</div>'`n" +
"                                        + '<div class=`"description meeting-note-content`">' + (item.MeetingNote || '') + '</div>'`n" +
"                                    + '</div>'`n" +
"                                + '</div>'`n" +
"                            + '</div>'`n" +
"                        + '</div>';`n" +
"                        `$container.append(html);`n" +
"                    });`n" +
"                }`n" +
"            },`n" +
"            error: function () {`n" +
"                `$('#dvGBMeetingList').empty();`n" +
"            }`n" +
"        });`n" +
"    }`n" +
"`n" +
"    `$(document).ready(function () {`n" +
"        // Init Action buttons on load based on ViewBag.Status`n" +
"        updateActionButtonsUI('@ViewBag.Status');`n" +
"    });`n" +
"</script>"

$content = $content -replace $pattern, $replacement
[IO.File]::WriteAllText('d:\Project\ASP .Net Project\BCG\Complaint\BCGAdminComplaint\BCGAdminComplaint\Views\ComplaintDetails\DetailsNew.cshtml', $content, [System.Text.Encoding]::UTF8)
