function ShowComments(response, current_user) {
    console.log(response);
    $("#comment-list").children().remove();
    $(".comment-number-" + mapIndex).text(response.length);
    for (var i = 0; i < response.length; i++) {
        
        if (response[i].PicOfCommento == null) {
            $('#comment-list').append('<img src="../images/profile_placeholder.png" class= "rounded-circle img-fluid mr-3"' +
                ' style = "width:3rem;" alt = "profile"> ');
        } else {
            $('#comment-list').append('<img src="data:image/jpeg;base64,' + response[i].PicOfCommento +
                '"alt="Circle Image" style= "width:2.5rem" class= "rounded-circle img-fluid mr-3">');
        }


        $("#comment-list").append(
            '<h5 class="d-inline-block" style="margin-top:-3px">' + response[i].User.FirstName + ' ' + response[i].User.LastName + '</h5> <br>' +
            '<span class="d-inline-block" style = "margin-left:4rem; margin-top:-3px">' + response[i].CommentContent + '</span> ');

        if (response[i].CommentedBy == current_user) {
            $('#comment-list').append('<button class="delete-comment-btn btn btn-sm btn-social btn-link float-right"  data-toggle="modal"' +
                'data-target="#deleteComment" value="' + response[i].Id + '" onclick="setCommentId(this.value)">' +
                '<i class="material-icons text-rose">delete</i> </button>');
        }

        $("#comment-list").append("<br />");
    }
}