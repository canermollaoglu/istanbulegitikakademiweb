var UploadSupport;
(function (UploadSupport) {

    var FileUploader = (function () {
        function FileUploader() {
            this.container = null;
            this.containerId = "";
            this.preview = null;
            this.fileInput = null;
            this.fileReader = new FileReader();
            this.base64Content = "";
            this.extension = "";
            this.validExtensions = ["png", "jpg", "jpeg"];
        }

        FileUploader.prototype.set = function (settings) {
            this.container = document.getElementById(settings.container);
            this.containerId = settings.container;
            this.fileInput = document.querySelector(`#${settings.container} input[type="file"]`);
            this.container.onclick = () => { this.fileInput.click(); };
            this.fileInput.onchange = (e) => {
                if (e.target.files.length > 0) {
                    var selectedFile = e.target.files[0];
                    var splitted = selectedFile.name.split(".");
                    this.extension = splitted[splitted.length - 1];
                    this.fileReader.readAsDataURL(selectedFile);
                }
            };
            this.fileReader.onload = (e) => {
                if (this.fileReader.DONE) {
                    var result = e.target.result;
                    this.base64content = result;
                    this.preview.setAttribute("src", result);
                    var afterPreview = document.querySelector(`#${this.containerId} .img-after-preview`);
                    afterPreview.style.display = "block";
                }
            };
            this.preview = document.getElementById(settings.preview);
            if (settings.validExtensions)
                validExtensions = settings.validExtensions;
        }
        FileUploader.prototype.getFile = function () {
            return {
                base64content: this.base64content,
                extension: this.extension
            }
        }
        FileUploader.prototype.validateExtension = function () {
            if (validExtensions.indexOf(extension) == -1)
                return false;
            return true;
        }

        return FileUploader;
    }());
    UploadSupport.FileUploader = FileUploader;
})(UploadSupport || (UploadSupport = {}));