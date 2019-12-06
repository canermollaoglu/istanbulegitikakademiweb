var AlertSupport;
(function (AlertSupport) {

    var ResultAlert = (function () {
        function ResultAlert() {
            this.alertContainer = $("#div-alert-container");
            this.alertIcon = $("#alert-icon");
            this.alertMessageList = $("#alert-message-list");
            this.alertSingleMessage = $("#alert-single-message");
            this.btnDismissAlert = $("#btn-dismiss-alert");
            this.btnDismissAlert.on("click", () => { this.alertContainer.css("display", "none"); });
        }

        // options = { success: <bool>, errors: [], message: <string>, redirectElement: {content: <string>, link: <string>} }
        ResultAlert.prototype.display = function (options) {
            this.alertMessageList.html("");
            this.alertSingleMessage.html("");
            buildMessage(this.alertSingleMessage, options);
            if (options.success) {
                addRemoveClasses(this.alertContainer, this.alertIcon, true);
            } else {
                addRemoveClasses(this.alertContainer, this.alertIcon, false);
                for (var i = 0; i < options.errors.length; i++)
                    this.alertMessageList.append(`<li>${options.errors[i]}</li>`);
            }

            this.alertContainer.css("display", "block");
        }

        function buildMessage(element, options) {
            if (options.message) {
                element.html(options.message);
                if (options.redirectElement) {
                    if (!options.redirectElement.content) options.redirectElement.content = "tıklayınız";
                    if (!options.redirectElement.link) options.redirectElement.link = "";
                    var link = `<a href="${options.redirectElement.link}">${options.redirectElement.content}</a>`;

                    if (element.html().indexOf("{link}") != -1) {
                        var messageContent = element.html();
                        messageContent = messageContent.replace("{link}", link);
                        element.html(messageContent);
                    }
                    else
                        element.append(` ${link}`);
                }
            }
        }
        function addRemoveClasses(container, icon, success) {
            if (success) {
                container.removeClass("alert-danger");
                container.addClass("alert-success");
                icon.removeClass("text-danger");
                icon.addClass("text-success");
                icon.removeClass("fa-exclamation-triangle");
                icon.addClass("fa-thumbs-up");
            } else {
                container.removeClass("alert-success");
                container.addClass("alert-danger");
                icon.removeClass("text-success");
                icon.addClass("text-danger");
                icon.removeClass("fa-thumbs-up");
                icon.addClass("fa-exclamation-triangle");
            }
        }

        return ResultAlert;
    }());
    AlertSupport.ResultAlert = ResultAlert;

    //var DeleteConfirm = (function () {
    //    function DeleteConfirm() {
    //        this.modalConfirmationTrigger = $("#modal-confirmation-trigger");
    //        this.modalConfirmation = $("#modal-confirmation");
    //        this.modalConfirmationTitle = $("#modal-confirmation-title");
    //        this.modalConfirmationBodyText = $("#modal-confirmation-body-text");
    //        this.modalBtnCancel = $("#modal-btn-cancel");
    //        this.modalBtnConfirm = $("#modal-btn-confirm");
    //    }

    //    // options = { confirmUrl: <string>, modalOptions: { title: <string>, bodyText: <string>, cancelText: <string>, confirmText: <string> } }
    //    DeleteConfirm.prototype.show = function (options) {
    //        this.modalBtnConfirm.attr("href", options.confirmUrl);
    //        if (options.modalOptions) {
    //            var o = options.modalOptions;
    //            if (o.title)
    //                this.modalConfirmationTitle.html(o.title);
    //            if (o.bodyText)
    //                this.modalConfirmationBodyText.html(o.bodyText);
    //            if (o.cancelText)
    //                this.modalBtnCancel.html(o.cancelText);
    //            if (o.confirmText)
    //                this.modalBtnConfirm.html(o.confirmText);
    //        }

    //        this.modalConfirmationTrigger.click();
    //    }

    //    return DeleteConfirm;
    //}());
    //AlertSupport.DeleteConfirm = DeleteConfirm;
})(AlertSupport || (AlertSupport = {}));