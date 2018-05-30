
        function savedInfo() {

          

            var panel = $(`


            <div class="j-flex-row j-secondary-header" >

                        <div class="alert alert-success" role="alert" style="margin:0px;width:100%">
                            <strong>Uloženo.</strong> Veškeré provedené změny byly úspěšně uloženy do databáze.
                        </div>
                
            </div>


`);

            panel.css({
                "display": "none",                
            });

            $("#headerPanel").append(panel);
            panel.slideDown("slow").delay(2000).slideUp("slow");

            $(":submit").blur();/* After submit the submit button had still focus efect*/
        }
