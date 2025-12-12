-- Create violations_requests table to track violation search requests
CREATE TABLE IF NOT EXISTS public.violations_requests (
    id UUID DEFAULT uuid_generate_v4() PRIMARY KEY,
    company_id UUID,
    vehicle_count INTEGER DEFAULT 0 NOT NULL,
    requests_count INTEGER DEFAULT 0 NOT NULL,
    finders_count INTEGER DEFAULT 0 NOT NULL,
    request_datetime TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    violations_found INTEGER DEFAULT 0 NOT NULL,
    requestor VARCHAR(255),

    CONSTRAINT fk_violations_requests_company
        FOREIGN KEY (company_id)
        REFERENCES public.companies (id)
        ON DELETE SET NULL
);

-- Add indexes for common query fields
CREATE INDEX IF NOT EXISTS idx_violations_requests_company_id ON public.violations_requests (company_id);
CREATE INDEX IF NOT EXISTS idx_violations_requests_request_datetime ON public.violations_requests (request_datetime DESC);

-- Add comments for clarity
COMMENT ON TABLE public.violations_requests IS 'Tracks violation search requests made through the API';
COMMENT ON COLUMN public.violations_requests.id IS 'Unique identifier for the request record.';
COMMENT ON COLUMN public.violations_requests.company_id IS 'Foreign key to the companies table, indicating which company this request was for (null if request was for specific vehicles).';
COMMENT ON COLUMN public.violations_requests.vehicle_count IS 'Number of vehicles processed in this request.';
COMMENT ON COLUMN public.violations_requests.requests_count IS 'Number of individual violation search requests made.';
COMMENT ON COLUMN public.violations_requests.finders_count IS 'Number of finders used in this request.';
COMMENT ON COLUMN public.violations_requests.request_datetime IS 'Timestamp when the request was made.';
COMMENT ON COLUMN public.violations_requests.violations_found IS 'Total number of violations found in this request.';
COMMENT ON COLUMN public.violations_requests.requestor IS 'Identifier of who made the request (e.g., user email, API key, system).';

