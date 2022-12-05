using System.Collections.Generic;

namespace PurchaselyRuntime
{
	public class Event
	{
		public string name;
		public string abTestId;
		public string abTestVariantId;
		public string anonymousUserId;
		public string appInstalledAt;
		public string appInstalledAtMs;
		public string audienceId;
		public string cancellationReason;
		public string cancellationReasonId;
		public List<Carousel> carousels;
		public string contentId;
		public string deeplinkIdentifier;
		public string device;
		public string displayedPresentation;
		public string errorMessage;
		public string eventCreatedAt;
		public long eventCreatedAtMs;
		public string eventName;
		public string language;
		public string linkIdentifier;
		public string osVersion;
		public long paywallRequestDurationInMs;
		public string placementId;
		public string plan;
		public string planChangeType;
		public string previousSelectedPlan;
		public string previousSelectedPresentation;
		public List<Plan> purchasablePlans;
		public List<Subscription> runningSubscriptions;
		public string screenDisplayedAt;
		public long screenDisplayedAtMs;
		public long screenDuration;
		public string sdkVersion;
		public string selectedPlan;
		public string selectedPresentation;
		public string selectedProduct;
		public int sessionCount;
		public long sessionDuration;
		public string sourceIdentifier;
		public string type;
		public string userId;

		public class Carousel
		{
			public int defaultSlide;/* compiled code */
			public bool isCarouselAutoPlaying;
			public int numberOfSlides;
			public int previousSlide;
			public int selectedSlide;
		}

		public class Plan
		{
			public string customerCurrency;
			public string discountPercentageComparisonToReferent;
			public float discountPriceComparisonToReferent;
			public string discountReferent;
			public int duration;
			public int freeTrialDuration;
			public string freeTrialPeriod;
			public bool hasFreeTrial;
			public int introDuration;
			public string introPeriod;
			public float introPriceInCustomerCurrency;
			public bool isDefault;
			public string period;
			public float priceInCustomerCurrency;
			public string purchaselyPlanId;
			public string store;
			public string storeCountry;
			public string storeProductId;
			public string type;
		}
		
		public class Subscription
		{
			public string plan;
			public string product;
		}
	}
}