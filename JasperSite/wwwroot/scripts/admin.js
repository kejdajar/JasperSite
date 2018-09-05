window.$=window.jQuery = require("../../node_modules/jquery/dist/jquery.js");
require("../../node_modules/popper.js/dist/umd/popper.js");
require("../../node_modules/bootstrap/dist/js/bootstrap.js");
require("../../node_modules/bootstrap/dist/css/bootstrap.css");
require("../../node_modules/font-awesome/css/font-awesome.css");
require("../../Areas/Admin/Content/Styles/design.css");
require("../../Areas/Admin/Content/Styles/simple-sidebar.css");
window.savedInfo = require("../../Areas/Admin/Content/Scripts/SavedInfoPanel.js");
window.UnobtrusiveValidationFix = require("../../Areas/Admin/Content/Scripts/PartialViewUnobtrusiveFix.js");

require("../../node_modules/jquery-ajax-unobtrusive/jquery.unobtrusive-ajax.js");
require("../../node_modules/jquery-validation/dist/jquery.validate.js");
require("../../node_modules/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js");