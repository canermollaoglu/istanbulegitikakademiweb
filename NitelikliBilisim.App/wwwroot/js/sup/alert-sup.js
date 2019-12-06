var AlertSupport;
(function (AlertSupport) {

    var Builder = (function () {
        function Builder() {
            this.alertContainer = $("#div-alert-container");
            this.alertIcon = $("#alert-icon");
            this.alertMessageList = $("#alert-message-list");
            this.alertSingleMessage = $("#alert-single-message");
            this.btnDismissAlert = $("#btn-dismiss-alert");
            this.btnDismissAlert.on("click", () => { this.alertContainer.css("display", "none"); });
        }

        // options = { success: <bool>, errors: [], message: <string>, redirectElement: {content: <string>, link: <string>} }
        Builder.prototype.display = function (options) {
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

        return Builder;
    }());
    AlertSupport.Builder = Builder;
})(AlertSupport || (AlertSupport = {}));