import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'fetchdata',
    templateUrl: './fetchdata.component.html'
})
export class FetchDataComponent {
    public userInfo: UserInfoData;

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        http.get(baseUrl + 'api/SampleData/UserInfo').subscribe(result => {
            this.userInfo = result.json() as UserInfoData;
        }, error => console.error(error));
    }
}

interface UserInfoData {
    sub: string;
    name: string;
    nickname: string;
    given_name: string;
    family_name: string;
    gender: string;
    birthdate: string;
    website: string;
    email: string;
    address: Address;
    phone_number: string;
    organization: Organization;
}

interface Address {
    street_address: string;
    locality: string;
    region: string;
    postal_code: string;
    country: string;
}

interface Organization {
    id: string;
    name: string;
}
