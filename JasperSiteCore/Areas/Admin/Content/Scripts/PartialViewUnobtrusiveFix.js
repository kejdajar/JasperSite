   
    // In partial views, unobtrusive validation doesn't work by default - validator has to be resetted
    // Before first POST, client side validation works - after first post, returned partial view is not validated again
    // --> Now after first POST all validation works as expected

 function UnobtrusiveValidationFix(formSelector) {

            $(formSelector).submit(function () {
                var form = $(formSelector)
                    .removeData("validator") /* added by the raw jquery.validate plugin */
                    .removeData("unobtrusiveValidation");  /* added by the jquery unobtrusive plugin*/
                $.validator.unobtrusive.parse(form);
            });

        } 