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
        }

        FileUploader.prototype.set = function (settings) {
            this.container = document.getElementById(settings.container);
            this.containerId = settings.container;
            this.fileInput = document.querySelector(`#${settings.container} input[type="file"]`);
            this.container.onclick = container_onClick;
            this.fileInput.onchange = fileInput_onChange;
            this.fileReader.onload = fileReader_onLoad;
            this.preview = document.getElementById(settings.preview);

        }
        FileUploader.prototype.getFile = function () {
            return {
                base64content: this.base64Content,
                extension: this.extension
            }
        }
        FileUploader.prototype.validateExtension = function () {

        }

        function container_onClick() {
            this.fileInput.onclick();
        }
        function fileInput_onChange(e) {
            if (e.target.files.length > 0) {
                var selectedFile = e.target.files[0];
                var splitted = selectedFile.name.split(".");
                this.extension = splitted[splitted.length - 1];
                this.fileReader.readAsDataURLL(selectedFile);
            }
        }
        function fileReader_onLoad(e) {
            if (this.fileReader.DONE) {
                var result = e.target.result;
                this.base64content = result;
                this.preview.setAttribute("src", result);
                var afterPreview = document.querySelector(`#${this.containerId} .img-after-preview`);
                afterPreview.style.display = "block";
            }
        }
        return FileUploader;
    }());
    UploadSupport.FileUploader = FileUploader;
})(UploadSupport || (UploadSupport = {}));