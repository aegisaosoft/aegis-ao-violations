-- Create violations table based on HuurApi.Models.ParkingViolation structure
-- with company_id foreign key to companies table

-- Drop table if exists (for development/testing)
-- DROP TABLE IF EXISTS public.violations;

CREATE TABLE IF NOT EXISTS public.violations (
	id uuid DEFAULT uuid_generate_v4() NOT NULL,
	company_id uuid NOT NULL,
	citation_number varchar(255) NULL,
	notice_number varchar(255) NULL,
	provider int DEFAULT 0 NOT NULL,
	agency varchar(255) NULL,
	address text NULL,
	tag varchar(50) NULL, -- License plate
	state varchar(10) NULL,
	issue_date timestamp NULL,
	start_date timestamp NULL,
	end_date timestamp NULL,
	amount numeric(10, 2) DEFAULT 0.00 NOT NULL,
	currency varchar(3) NULL,
	payment_status int DEFAULT 0 NOT NULL,
	fine_type int DEFAULT 0 NOT NULL,
	note text NULL,
	link text NULL,
	is_active bool DEFAULT true NOT NULL,
	created_at timestamp DEFAULT CURRENT_TIMESTAMP NOT NULL,
	updated_at timestamp DEFAULT CURRENT_TIMESTAMP NOT NULL,
	CONSTRAINT violations_pkey PRIMARY KEY (id),
	CONSTRAINT fk_violations_company FOREIGN KEY (company_id) REFERENCES public.companies(id) ON DELETE CASCADE
);

-- Create indexes for better query performance
CREATE INDEX idx_violations_company_id ON public.violations USING btree (company_id);
CREATE INDEX idx_violations_tag ON public.violations USING btree (tag);
CREATE INDEX idx_violations_state ON public.violations USING btree (state);
CREATE INDEX idx_violations_issue_date ON public.violations USING btree (issue_date);
CREATE INDEX idx_violations_notice_number ON public.violations USING btree (notice_number);
CREATE INDEX idx_violations_citation_number ON public.violations USING btree (citation_number);
CREATE INDEX idx_violations_is_active ON public.violations USING btree (is_active);
CREATE INDEX idx_violations_created_at ON public.violations USING btree (created_at DESC);
CREATE INDEX idx_violations_company_tag ON public.violations USING btree (company_id, tag);
CREATE INDEX idx_violations_company_state ON public.violations USING btree (company_id, state);

-- Create trigger to update updated_at timestamp
CREATE OR REPLACE FUNCTION update_violations_updated_at()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER update_violations_updated_at
    BEFORE UPDATE ON public.violations
    FOR EACH ROW
    EXECUTE FUNCTION update_violations_updated_at();

-- Column comments
COMMENT ON TABLE public.violations IS 'Parking violations table linked to companies';
COMMENT ON COLUMN public.violations.id IS 'Unique violation identifier (UUID)';
COMMENT ON COLUMN public.violations.company_id IS 'Foreign key reference to companies table';
COMMENT ON COLUMN public.violations.citation_number IS 'Citation number from the issuing agency';
COMMENT ON COLUMN public.violations.notice_number IS 'Notice number from the issuing agency';
COMMENT ON COLUMN public.violations.provider IS 'Provider identifier (integer)';
COMMENT ON COLUMN public.violations.agency IS 'Agency that issued the violation';
COMMENT ON COLUMN public.violations.address IS 'Address where the violation occurred';
COMMENT ON COLUMN public.violations.tag IS 'License plate number';
COMMENT ON COLUMN public.violations.state IS 'State code where violation occurred';
COMMENT ON COLUMN public.violations.issue_date IS 'Date when the violation was issued';
COMMENT ON COLUMN public.violations.start_date IS 'Start date for the violation period';
COMMENT ON COLUMN public.violations.end_date IS 'End date for the violation period';
COMMENT ON COLUMN public.violations.amount IS 'Fine amount';
COMMENT ON COLUMN public.violations.currency IS 'Currency code (ISO 4217)';
COMMENT ON COLUMN public.violations.payment_status IS 'Payment status (integer code)';
COMMENT ON COLUMN public.violations.fine_type IS 'Type of fine (integer code)';
COMMENT ON COLUMN public.violations.note IS 'Additional notes about the violation';
COMMENT ON COLUMN public.violations.link IS 'Link to violation details or payment page';
COMMENT ON COLUMN public.violations.is_active IS 'Whether the violation record is active';

-- Permissions
ALTER TABLE public.violations OWNER TO alex;
GRANT ALL ON TABLE public.violations TO alex;

