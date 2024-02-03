function exportTableToExcel(tableID, filename = '') {
    var tableExport = document.getElementById(tableID);
    var html = tableExport.outerHTML;
    var dataType = 'application/vnd.ms-excel';
    while (html.indexOf('ç') != -1) html = html.replace('ç', '&ccedil;');
    while (html.indexOf('ğ') != -1) html = html.replace('ğ', '&#287;');
    while (html.indexOf('ı') != -1) html = html.replace('ı', '&#305;');
    while (html.indexOf('ö') != -1) html = html.replace('ö', '&ouml;');
    while (html.indexOf('ş') != -1) html = html.replace('ş', '&#351;');
    while (html.indexOf('ü') != -1) html = html.replace('ü', '&uuml;');

    while (html.indexOf('Ç') != -1) html = html.replace('Ç', '&Ccedil;');
    while (html.indexOf('Ğ') != -1) html = html.replace('Ğ', '&#286;');
    while (html.indexOf('İ') != -1) html = html.replace('İ', '&#304;');
    while (html.indexOf('Ö') != -1) html = html.replace('Ö', '&Ouml;');
    while (html.indexOf('Ş') != -1) html = html.replace('Ş', '&#350;');
    while (html.indexOf('Ü') != -1) html = html.replace('Ü', '&Uuml;');

    //  window.open('data:application/vnd.ms-excel,' + encodeURIComponent(html));

    // Specify file name
    filename = filename ? filename + '.xls' : 'excel_data.xlsx';

    // Create download link element
    downloadLink = document.createElement("a");

    document.body.appendChild(downloadLink);

    if (navigator.msSaveOrOpenBlob) {
        var blob = new Blob(["\ufeff", encodeURIComponent(html)], {
            type: dataType
        });
        navigator.msSaveOrOpenBlob(blob, filename);
    } else {
        // Create a link to the file
        downloadLink.href = 'data:' + dataType + ', ' + encodeURIComponent(html);

        // Setting the file name
        downloadLink.download = filename;

        //triggering the function
        downloadLink.click();
    }


}