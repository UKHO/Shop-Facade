import http from 'k6/http';
import { check } from 'k6';
import { htmlReport } from "https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js";
import { textSummary } from "https://jslib.k6.io/k6-summary/0.0.1/index.js";
import { URL } from 'https://jslib.k6.io/url/1.0.0/index.js';

let licenceIdList = [1, 2, 3, 4, 5, 6, 7, 8]; // Update this array with the list of actual LicenceId's present in the SharePoint list.
var Config = JSON.parse(open('./../config.json'));
const headers = {
    Authorization: `Bearer ${Config.Token}`,
};

export const options = {
    discardResponseBodies: true,
    scenarios: {
        ConstantRequestRate: {
            executor: 'constant-arrival-rate',
            exec: 'ConstantRequestRate',
            rate: Config.RequestRate,
            timeUnit: '1h',
            duration: '30m',
            preAllocatedVUs: 5,
            maxVUs: 30,
        },
    }
};

export function ConstantRequestRate() {
    if (licenceIdList.length === 0) {
        console.error("Licence ID list is empty.");
        return;
    }

    const url = new URL(Config.BaseUrl + Config.Endpoint);
    let apiUrl = url.toString().replace("licenceId", licenceIdList[Math.floor(Math.random() * licenceIdList.length)]);

    const res = http.get(apiUrl, { headers }, { tags: { my_custom_tag: 'ConstantRequestRate' } });

    check(res, {
        'Status is 200': (r) => r.status === 200,
    });

    console.log("Status code: " + res.status);
}

export function teardown() {
    const eventEndDate = new Date(Date.now());
    console.log("End time: " + eventEndDate.toUTCString());
}

// Create HTML report
export function handleSummary(data) {
    const timestamp = new Date().toISOString().substr(0, 19).replace(/(:|-)/g, "").replace("T", "_");
    return {
        [`./../Summary/TestResult_${timestamp}.html`]: htmlReport(data),
        stdout: textSummary(data, { indent: " ", enableColors: true }),
        [`./../Summary/TestResult_${timestamp}.json`]: JSON.stringify(data),
    };
}
