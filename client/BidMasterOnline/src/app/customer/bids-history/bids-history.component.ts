import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { DataTableOptionsModel } from 'src/app/models/dataTableOptionsModel';
import { AuthService } from 'src/app/services/auth.service';
import { UsersDeepLinkingService } from 'src/app/services/users-deep-linking.service';
import { UsersService } from 'src/app/services/users.service';

@Component({
  selector: 'app-bids-history',
  templateUrl: './bids-history.component.html'
})
export class BidsHistoryComponent implements OnInit {

  options: DataTableOptionsModel;

  allowOpenBidsHistory: boolean = false;

  userId: string;

  constructor(private readonly usersService: UsersService,
    private readonly authService: AuthService,
    private usersDeepLinkingService: UsersDeepLinkingService,
    private readonly toastrService: ToastrService) {

  }

  async ngOnInit(): Promise<void> {
    var userId = await this.usersDeepLinkingService.getUserId();

    if (userId == null) {
      this.toastrService.error('Invalid query params.', 'Error');
      return;
    }

    this.userId = userId;

    var user = this.authService.user;

    if (user.userId == this.userId) {
      this.allowOpenBidsHistory = true;

      this.options = this.usersService.getUsersBidsDataTableOptions();
    }
  }

  getDataTableApiUrl() {
    return this.usersService.getUsersBidsDataTableApiUrl(this.userId);
  }
}