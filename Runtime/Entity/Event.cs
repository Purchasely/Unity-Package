using System.Collections.Generic;

namespace PurchaselyRuntime
{
	public class Event
	{
		public string name;
		public string ab_test_id;
		public string ab_test_variant_id;
		public string anonymous_user_id;
		public string app_installed_at;
		public string app_installed_at_ms;
		public string audience_id;
		public string cancellation_reason;
		public string cancellation_reason_id;
		public List<Carousel> carousels;
		public string content_id;
		public string deeplink_identifier;
		public string device;
		public string displayed_presentation;
		public string error_message;
		public string event_created_at;
		public long event_created_at_ms;
		public string event_name;
		public string language;
		public string link_identifier;
		public string os_version;
		public long paywall_request_duration_in_ms;
		public string placement_id;
		public string plan;
		public string plan_change_type;
		public string previous_selected_plan;
		public string previous_selected_presentation;
		public List<Plan> purchasable_plans;
		public List<Subscription> running_subscriptions;
		public string screen_displayed_at;
		public long screen_displayed_at_ms;
		public long screen_duration;
		public string sdk_version;
		public string selected_plan;
		public string selected_presentation;
		public string selected_product;
		public int session_count;
		public long session_duration;
		public string source_identifier;
		public string type;
		public string user_id;

		public class Carousel
		{
			public int default_slide;
			public bool is_carousel_auto_playing;
			public int number_of_slides;
			public int previous_slide;
			public int selected_slide;
		}

		public class Plan
		{
			public string customer_currency;
			public string discount_percentage_comparison_to_referent;
			public float discount_price_comparison_to_referent;
			public string discount_referent;
			public int duration;
			public int free_trial_duration;
			public string free_trial_period;
			public bool has_free_trial;
			public int intro_duration;
			public string intro_period;
			public float intro_price_in_customer_currency;
			public bool is_default;
			public string period;
			public float price_in_customer_currency;
			public string purchasely_plan_id;
			public string store;
			public string store_country;
			public string store_product_id;
			public string type;
		}
		
		public class Subscription
		{
			public string plan;
			public string product;
		}
	}
}