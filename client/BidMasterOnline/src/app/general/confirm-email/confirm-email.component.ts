import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DeepLinkingService } from 'src/app/services/deep-linking.service';
import { UsersService } from 'src/app/services/users.service';

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
})
export class ConfirmEmailComponent implements OnInit {

  response: string;
  error: string;

  constructor(private readonly deepLinkingService: DeepLinkingService,
    private readonly usersService: UsersService) {

  }

  async ngOnInit(): Promise<void> {
    var userId = await this.deepLinkingService.getQueryParam('userId');

    this.usersService.confirmEmail(userId).subscribe(
      (response) => {
        this.response = response.message;
      },
      (error) => {
        this.error = error.error;
      }
    );
  }
}
