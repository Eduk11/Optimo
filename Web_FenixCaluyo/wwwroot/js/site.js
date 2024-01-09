//document.addEventListener("DOMContentLoaded", function () {
//    var tabla = document.getElementById("report_data");
//    var filas = tabla.getElementsByTagName("tr");

//    console.log("Número total de filas: " + filas.length);

//    for (var i = 11; i < filas.length; i++) {
//        filas[i].style.display = "none";
//    }
//});










$('#report').on('click', function () {
    const params = new URLSearchParams();
    //params.append('FechaCon', FechaCon);
    //params.append('TipoCon', Consulta);
    //params.append('SucAgeCon', '');
    //params.append('TetiCon', '');
    const url = GetRoute('api', 'ReporteVenta');
    window.location.href = url + '?' + params, {
        method: 'GET'
    };
    //window.location.href = url + '?' + params, {
    //    method: 'GET'
    //};
});